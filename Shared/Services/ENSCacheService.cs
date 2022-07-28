using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Lexplorer.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace Lexplorer.Services
{
    using DomainsDictionary = Dictionary<string, ENS.SourceType>;
    using AddressDomainsDictionary = Dictionary<string, Dictionary<string, ENS.SourceType>>;

    public class ENSCacheService : IDisposable
    {
        readonly RestClient _client;

        private readonly ConcurrentDictionary<string, DomainsDictionary> ensEntries = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly HashSet<string> ensAlreadyLookedUp = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private bool disableCaching { get; set; } = false; //for tests
        public void DisableCache() => disableCaching = true;
        public void EnableCache()
        {
            disableCaching = false;
            ensEntries.Clear();
            ensAlreadyLookedUp.Clear();
        }

        public ENSCacheService(IConfiguration config)
        {
            _client = new RestClient(config.GetSection("services:ENSGraph:endpoint").Value);
        }

        public ENSCacheService(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void AddDomains(string address, string[]? domainNames, ENS.SourceType sourceType)
        {
            if (disableCaching) return;
            if ((domainNames?.Length ?? 0) == 0) return;

            _ = ensEntries.AddOrUpdate(address,
                domainNames!.ToDictionary(x => x, x => sourceType),
                (key, oldvalue) =>
                {
                    //do not overwrite sourceType if already there
                    foreach (string domain in domainNames!)
                        oldvalue.TryAdd(domain, sourceType);
                    return oldvalue;
                });
        }

        public void AddLookupAddress(string domainName, string hexAddress)
        {
            AddDomains(hexAddress, new[] {domainName}, ENS.SourceType.Lookup);
        }

        public async Task<DomainsDictionary?> ReverseLookupAddress(string? address, CancellationToken cancellationToken = default)
        {
            if (address == null) return null;

            DomainsDictionary? retValue = null;
            if (!ensAlreadyLookedUp.Contains(address))
            {
                try
                {
                    var accounts = await ReverseLookup(new List<string>() { address }, cancellationToken);
                    if (!disableCaching)
                        _ = ensAlreadyLookedUp.Add(address);

                    var domainList = accounts?.FirstOrDefault<ENS.Account>()?.domains;
                    var domainNames = domainList?.Select(item => item.name).
                        OfType<string>() //skip null values
                        .ToArray();

                    AddDomains(address, domainNames, ENS.SourceType.ReverseLookup);
                    if ((disableCaching) && ((domainNames?.Length ?? 0) > 0))
                        retValue = domainNames!.ToDictionary(x => x, x => ENS.SourceType.ReverseLookup);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            if (!disableCaching)
                ensEntries.TryGetValue(address, out retValue);
            return retValue;
        }

        public async Task<List<ENS.Account>?> ReverseLookup(IList<string> addresses, CancellationToken cancellationToken = default)
        {
            var revLookupQuery = @"
            query revLookup(
                $addresses: [String]
            ){
                accounts(
                    where: {id_in: $addresses}
                ) {
                    id
                    domains {
                      id
                      name
                      labelName
                      labelhash
                      parent {
                        id
                      }
                    }
                }
            }
            ";

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = revLookupQuery,
                variables = new
                {
                    addresses
                }
            });
            var response = await _client.PostAsync(request, cancellationToken);
            JObject jresponse = JObject.Parse(response.Content!);
            JToken? jtoken = jresponse["data"]!["accounts"];
            return jtoken!.ToObject<List<ENS.Account>>()!;
        }

        public AddressDomainsDictionary? GetCachedENS(ref List<string>? addressList, AddressDomainsDictionary? existingENSDictionary = default)
        {
            if ((addressList?.Count ?? 0) == 0) return existingENSDictionary;

            Dictionary<string, DomainsDictionary>? retValue = null;
            if (existingENSDictionary != null)
                retValue = new(existingENSDictionary);
            for (int i = addressList!.Count - 1; i >= 0; i--)
            {
                var address = addressList[i];
                if (ensEntries.TryGetValue(address, out DomainsDictionary? dd))
                {
                    if (retValue == null)
                        retValue = new();
                    retValue.TryAdd(address, dd);
                    addressList.RemoveAt(i);
                }
                else if (ensAlreadyLookedUp.Contains(address))
                    addressList.RemoveAt(i);
            }
            return retValue;
        }

        public async Task<AddressDomainsDictionary?> ReverseLookupAddressList(List<string>? addressList,
            AddressDomainsDictionary? existingENSDictionary, CancellationToken cancellationToken = default)
        {
            if ((addressList?.Count ?? 0) == 0) return existingENSDictionary;

            //get the already cached ones, just to be sure - they'll get removed from addressList
            Dictionary<string, DomainsDictionary>? retValue = GetCachedENS(ref addressList, existingENSDictionary);
            if (addressList!.Count == 0)
                return retValue;

            var revLookedUp = await ReverseLookup(addressList, cancellationToken);
            //add the new found ones to the cache
            if (revLookedUp != null)
                foreach (var account in revLookedUp)
                {
                    if (account.id == null) continue;

                    var domainNames = account.domains?.Select(item => item.name).
                        OfType<string>() //skip null values
                        .ToArray();

                    AddDomains(account.id, domainNames, ENS.SourceType.ReverseLookup);
                }
            //add all to the list of the already looked up addresses and fill
            //retValue from the cache so that it contains all source types,
            //not just the ones just looked up
            foreach (var address in addressList)
            {
                ensAlreadyLookedUp.Add(address);
                if (ensEntries.TryGetValue(address, out DomainsDictionary? dd))
                {
                    if (retValue == null)
                        retValue = new();
                    retValue.TryAdd(address, dd);
                }
            }

            return retValue;
        }
    }
}
