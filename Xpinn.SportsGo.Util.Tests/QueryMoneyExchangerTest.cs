using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Models;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Tests
{
    [TestClass]
    public class QueryMoneyExchangerTest
    {
        [TestMethod]
        public async Task QueryMoneyExchanger_QueryMoneyExchange_ShouldQuery()
        {
            QueryMoneyExchanger queryExchanger = new QueryMoneyExchanger();

            YahooExchangeEntity entity = await queryExchanger.QueryMoneyExchange("COP", "USD");

            Assert.IsNotNull(entity);
        }
    }
}
