using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Lexplorer.Models;

namespace Lexplorer.Services
{
	public class LoopringPoolTokenCacheService
	{
        private readonly LoopringGraphQLService _loopringService;
        private readonly EthereumService _ethereumService;

        private readonly ConcurrentDictionary<string, LoopringPoolToken> poolTokensByTokenID = new ();
        private readonly ConcurrentDictionary<Tuple<string, string>, LoopringPoolToken> poolTokensByPairTokenIDs = new ();
        private readonly ConcurrentDictionary<string, LoopringPoolToken> poolTokensByPoolID = new();

        private bool disableCaching { get; set; } = false; //for tests
        public void DisableCache() => disableCaching = true;
        public void EnableCache()
        {
            disableCaching = false;
            poolTokensByTokenID.Clear();
            poolTokensByPairTokenIDs.Clear();
            poolTokensByPoolID.Clear();
        }

        public LoopringPoolTokenCacheService(LoopringGraphQLService loopringService, EthereumService ethereumService)
		{
			_loopringService = loopringService;
            _ethereumService = ethereumService;
        }

		private void AddCachedPoolToken(LoopringPoolToken token)
        {
            token.token!.name = $"AMM-{token.pair!.token0!.failSafeSymbol!.ToUpper()}-{token.pair!.token1!.failSafeSymbol!.ToUpper()}";
            token.token!.symbol = $"LP-{token.pair!.token0!.failSafeSymbol!.ToUpper()}-{token.pair!.token1!.failSafeSymbol!.ToUpper()}";
            token.token!.decimals = 8; //seems to be contant, see https://github.com/Loopring/protocols/blob/release_loopring_3.6.3/packages/loopring_v3/contracts/amm/PoolToken.sol
            if (disableCaching) return;

            poolTokensByTokenID.AddOrUpdate(token.token!.id!, token, (key, oldvalue) => token);
            if ((token.pair?.token0 != null) && (token.pair?.token1 != null))
			    poolTokensByPairTokenIDs.AddOrUpdate(new(token.pair!.token0!.id!, token.pair!.token1!.id!), token, (key, oldvalue) => token);
            if (token.pool?.id != null)
			    poolTokensByPoolID.AddOrUpdate(token.pool!.id!, token, (key, oldvalue) => token);
        }

        private LoopringPoolToken? GetCachedPoolToken(string token0ID, string token1ID)
        {
            LoopringPoolToken? token = null;
            Tuple<string, string> pairTokenTuple = new(token0ID, token1ID);
            if (!poolTokensByPairTokenIDs.TryGetValue(pairTokenTuple, out token))
            {
                //try other way around
                pairTokenTuple = new(token1ID, token0ID);
                poolTokensByPairTokenIDs.TryGetValue(pairTokenTuple, out token);
            }
            return token;
        }

        public async Task<LoopringPoolToken?> GetPoolToken(Pair pair, CancellationToken cancellationToken = default)
        {
            LoopringPoolToken? token = GetCachedPoolToken(pair.token0!.id!, pair.token1!.id!);
            if (token != null)
                return token;

            return await AddPoolToken(pair, cancellationToken);
        }

        public async Task<LoopringPoolToken?> GetPoolToken(Pool pool, CancellationToken cancellationToken = default)
        {
            LoopringPoolToken? token = null;
            if (poolTokensByPoolID.TryGetValue(pool.id!, out token))
                return token;

            return await AddPoolToken(pool, cancellationToken);
        }

        public async Task<LoopringPoolToken?> GetPoolToken(Swap swap, CancellationToken cancellationToken = default)
        {
            if (swap.pool != null)
                return await GetPoolToken(swap.pool);
            if (swap.pair != null)
                return await GetPoolToken(swap.pair);
            return await AddPoolToken(swap, cancellationToken);
        }

        public LoopringPoolToken? GetExistingPoolToken(Token token)
        {
            LoopringPoolToken? poolToken = null;
            poolTokensByTokenID.TryGetValue(token.id!, out poolToken);
            return poolToken;
        }

        public async Task<LoopringPoolToken?> GetPoolToken(Token token, CancellationToken cancellationToken = default)
        {
            LoopringPoolToken? poolToken = GetExistingPoolToken(token);
            if (poolToken != null)
                return poolToken;

            //now how to do this??? -> get any Remove transaction with this tokenID
            //but we have Removes with regular tokens as well, so only do this if token has no name
            if (!string.IsNullOrEmpty(token.name))
                return null;
            Remove? remove = await _loopringService.GetAnyRemoveWithTokenID(token!.id!, cancellationToken);
            if (remove?.pool == null)
                return null;
            return await AddPoolToken(remove.pool, cancellationToken);
        }

        private async Task<LoopringPoolToken?> AddPoolToken(Object theObject, CancellationToken cancellationToken = default)
        {
            LoopringPoolToken? token = null;
            Swap? swap = await _loopringService.GetSwapPairAndPool(theObject, cancellationToken);
            if ((swap != null) && (!poolTokensByPoolID.TryGetValue(swap.pool!.id!, out token)))
            {
                Token? poolToken = FindPoolToken(swap);
                if (poolToken == null) return null;
                token = new LoopringPoolToken();
                token.token = poolToken;
                token.pair = swap.pair;
                token.pool = swap.pool;
                AddCachedPoolToken(token);
            }
            return token!;
        }

        private Token? FindPoolToken(Swap swap)
        {
            if (swap.pool?.balances == null)
                return null;
            if ((swap.pair?.token0 == null) || (swap.pair?.token1 == null))
                return null;

            Token? poolToken = null;
            bool token0Found = false;
            bool token1Found = false;
            foreach (var balance in swap.pool.balances)
            {
                if (balance.token?.id == swap.pair.token0.id)
                    token0Found = true;
                else if (balance.token?.id == swap.pair.token1.id)
                    token1Found = true;
                else if (string.IsNullOrEmpty(balance.token?.symbol))
                    poolToken = balance.token!;
            }
            return ((token0Found) && (token1Found)) ? poolToken : null;
        }

    }
}
