using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using JsonSubTypes;
using Newtonsoft.Json;
using xUnitTests.Utils;
using Newtonsoft.Json.Linq;

namespace xUnitTests.LoopringGraphTests
{
    public partial class TestAccountTransactions
    {
        [Theory]
        [TestPriority(5)]
        [JsonFileData("AccountTransactions.json")]
        public void TestExport(JArray transactionsJArray)
        {
            IList<Transaction> transactions = transactionsJArray.ToObject<IList<Transaction>>()!;
            TranscationExportDefaultCSVFormat exporter = new TranscationExportDefaultCSVFormat();
            foreach (Transaction transaction in transactions)
            {
                exporter.WriteTransaction(transaction, fixture.testAccountId, (string line) =>
                    {
                        Assert.NotEmpty(line);
                        //todo: test the line, split the elements
                        //currently this test is only useful for debugging
                    });
            }
        }
    }
}
