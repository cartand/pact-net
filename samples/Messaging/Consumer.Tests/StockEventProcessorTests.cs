using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class StockEventProcessorTests
    {
        private readonly IMessagePactBuilderV3 messagePact;

        public StockEventProcessorTests(ITestOutputHelper output)
        {
            IMessagePactV3 v3 = MessagePact.V3("Stock Event Consumer", "Stock Event Producer", new PactConfig
            {
                PactDir = "../../../pacts/",
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                },
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            });

            this.messagePact = v3.UsingNativeBackend();
        }

        [Fact]
        public void ReceiveSomeStockEvents()
        {
            this.messagePact
                .ExpectsToReceive("some stock ticker events")
                .Given("A list of events is pushed to the queue")
                .WithMetadata("key", "valueKey")
                .WithJsonContent( new
                {
                    desired = new
                    {
                        deployments = new Dictionary<object, object> {
                                {Match.Type("6XKISmGMWynbwM52mxov6S"),
                                    new {
                                        id = Match.Type("6XKISmGMWynbwM52mxov6S"),
                                        appId =  Match.Type("amqp-dummy"),
                                        appVersion =  Match.Type("0.0.29"),
                                    }
                                }

                            },
                        deploymentsRemovals = Match.Type(new Dictionary<string, object> {
                                {"4JgEA5GCeqwVsu6Qada9XS",
                                    new {
                                        id = Match.Type("4JgEA5GCeqwVsu6Qada9XS"),
                                        appId = Match.Type("amqp-dummy"),
                                        appVersion = Match.Type("0.0.29"),
                                    }
                                }
                            })
                    }
                }
                )
                .Verify<JObject>(events =>
                {
                    
                });
        }
    }
}
