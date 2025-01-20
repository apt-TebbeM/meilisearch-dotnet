using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class FacetSearchTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private Index _indexForFaceting;

        private readonly TFixture _fixture;

        public FacetSearchTests(TFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            _indexForFaceting = await _fixture.SetUpIndexForFaceting("IndexForFaceting-SearchTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task BasicFacetSearch()
        {
            var results = await _indexForFaceting.FacetSearchAsync("genre");

            Assert.Equal(4, results.FacetHits.Count());
            Assert.Null(results.FacetQuery);
        }

        //[Fact] //these may or may not be required.
        //public async Task BasicFacetSearchWithNoFacet()
        //{
        //    var results = await _indexForFaceting.SearchFacetsAsync(null);

        //    results.FacetHits.Should().BeEmpty();
        //}

        //[Fact]
        //public async Task BasicFacetSearchWithEmptyFacet()
        //{
        //    var results = await _indexForFaceting.SearchFacetsAsync(string.Empty);

        //    results.FacetHits.Should().BeEmpty();
        //}

        [Fact]
        public async Task FacetSearchWithFilter()
        {
            var query = new FacetSearchQuery()
            {
                Filter = "genre = SF"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Single(results.FacetHits);
            Assert.Equal("SF", results.FacetHits.First().Value);
            Assert.Equal(2, results.FacetHits.First().Count);
            Assert.Null(results.FacetQuery);
        }

        [Fact]
        public async Task FacetSearchWithFilterWithSpaces()
        {
            var query = new FacetSearchQuery()
            {
                Filter = "genre = 'sci fi'"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Single(results.FacetHits);
            Assert.Equal("sci fi", results.FacetHits.First().Value);
            Assert.Equal(1, results.FacetHits.First().Count);
            Assert.Null(results.FacetQuery);
        }

        [Fact]
        public async Task FacetSearchWithFilterFacetIsNotNull()
        {
            var query = new FacetSearchQuery()
            {
                Filter = "genre IS NOT NULL"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Equal(4, results.FacetHits.Count());
            Assert.Equal("Action", results.FacetHits.First().Value);
            Assert.Equal(3, results.FacetHits.First().Count);
            Assert.Null(results.FacetQuery);
        }

        [Fact]
        public async Task FacetSearchWithMultipleFilter()
        {
            var newFilters = new Settings
            {
                FilterableAttributes = new string[] { "genre", "id" },
            };
            var task = await _indexForFaceting.UpdateSettingsAsync(newFilters);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _indexForFaceting.WaitForTaskAsync(task.TaskUid);

            var query = new FacetSearchQuery()
            {
                Filter = "genre = SF AND id != 13"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Single(results.FacetHits);
            Assert.Equal("SF", results.FacetHits.First().Value);
            Assert.Equal(1, results.FacetHits.First().Count);
            Assert.Null(results.FacetQuery);
        }

        [Fact]
        public async Task FacetSearchWithFilterFacetIsNull()
        {
            var query = new FacetSearchQuery()
            {
                Filter = "genre IS NULL"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Empty(results.FacetHits);
            Assert.Null(results.FacetQuery);
        }

        [Fact]
        public async Task FacetSearchWithFacetQuery()
        {
            var query = new FacetSearchQuery()
            {
                FacetQuery = "SF"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Single(results.FacetHits);
            Assert.Equal("SF", results.FacetHits.First().Value);
            Assert.Equal(2, results.FacetHits.First().Count);
            results.FacetQuery.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task FacetSearchWithFacetQueryWithSpaces()
        {
            var query = new FacetSearchQuery()
            {
                FacetQuery = "sci fi"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Single(results.FacetHits);
            Assert.Equal("sci fi", results.FacetHits.First().Value);
            Assert.Equal(1, results.FacetHits.First().Count);
            results.FacetQuery.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task FacetSearchWithLooseFacetQuery()
        {
            var query = new FacetSearchQuery()
            {
                FacetQuery = "s"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Equal(2, results.FacetHits.Count());
            Assert.Equal("sci fi", results.FacetHits.First().Value);
            Assert.Equal(1, results.FacetHits.First().Count);
            results.FacetQuery.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task FacetSearchWithLooseQuery()
        {
            var query = new FacetSearchQuery()
            {
                Query = "s"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Equal(3, results.FacetHits.Count());
            Assert.Contains(results.FacetHits, x => x.Value.Equals("Action") && x.Count == 1);
            Assert.Contains(results.FacetHits, x => x.Value.Equals("SF") && x.Count == 2);
            Assert.Contains(results.FacetHits, x => x.Value.Equals("sci fi") && x.Count == 1);
            Assert.Null(results.FacetQuery);
        }

        [Fact]
        public async Task FacetSearchWithMultipleQueryAndLastMatchingStrategy()
        {
            var query = new FacetSearchQuery()
            {
                Query = "action spider man",
                MatchingStrategy = "last"
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Single(results.FacetHits);
            results.FacetHits.First().Count.Should().Be(3);
            Assert.Null(results.FacetQuery);
        }

        [Fact]
        public async Task FacetSearchWithMultipleQueryAndAllMatchingStrategy()
        {
            var query = new FacetSearchQuery()
            {
                Query = "action spider man",
                MatchingStrategy = "all",
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Single(results.FacetHits);
            results.FacetHits.First().Count.Should().Be(1);
            Assert.Null(results.FacetQuery);
        }

        [Fact]
        public async Task FacetSearchWithMultipleQueryAndAllMatchingStrategyAndAttributesToSearchOn()
        {
            var query = new FacetSearchQuery()
            {
                Query = "spider man",
                MatchingStrategy = "all",
                AttributesToSearchOn = new[] { "name" }
            };
            var results = await _indexForFaceting.FacetSearchAsync("genre", query);

            Assert.Single(results.FacetHits);
            results.FacetHits.First().Count.Should().Be(1);
            Assert.Null(results.FacetQuery);
        }
    }
}
