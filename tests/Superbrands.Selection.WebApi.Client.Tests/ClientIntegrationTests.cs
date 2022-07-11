using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Superbrands.Selection.WebApi.Client.DTOs;
using Xunit;

namespace Superbrands.Selection.WebApi.Client.Tests
{
    public class ClientIntegrationTests
    {
        private readonly IOptions<SelectionClientConfiguration> _configuration =
            new OptionsWrapper<SelectionClientConfiguration>(new SelectionClientConfiguration
            {
                BaseUri = new Uri("http://localhost:5000/api/")
            });

        private readonly ILogger<SelectionsClient> _logger = new Logger<SelectionsClient>(new LoggerFactory());

       // [Fact]
        public async Task CreateSelection_SelectionCreated()
        {
            var client = new SelectionsClient(_configuration, _logger);

            var selection = await client.CreateSelection("test selection", false, 1, 1, CancellationToken.None);

            selection.SelectionId.Should().BeGreaterThan(0);
        }

       // [Fact]
        public async Task EditSelection_SelectionEdited()
        {
            var client = new SelectionsClient(_configuration, _logger);
            await client.EditSelection(13, "test selection edited", 1, CancellationToken.None);
        }

     //   [Fact]
        public async Task GetSelection_Got()
        {
            var client = new SelectionsClient(_configuration, _logger);

            var selection = await client.GetSelection(13, CancellationToken.None);

            selection.Should().NotBeNull();
        }

     //   [Fact]
        public async Task GetSelections_Got()
        {
            var client = new SelectionsClient(_configuration, _logger);

            var selection = await client.GetSelections(1);

            selection.Should().NotBeNullOrEmpty();
        }

    //    [Fact]
        public async Task AddProductToSelection_Added()
        {
            var client = new SelectionsClient(_configuration, _logger);

            await client.AddProductToSelection(1, "skuskusku", 1, new List<ColorModel>
                {
                    new ColorModel{Sku = "skuOfColor-size"}
                }, CancellationToken.None);
            
        }

     //   [Fact]
        public async Task RemoveProductFromSelection_Removed()
        {
            var client = new SelectionsClient(_configuration, _logger);

            await client.RemoveProductFromSelection(1, 1, 1, CancellationToken.None);
            
        }
    }
}