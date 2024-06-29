using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using SharpVids.Data;

namespace SharpVids.Tests.Unit;

public sealed class RawVideoDbTests
{
    public RawVideoDbTests()
    {
        _connectionFactory.Create()
            .Returns(_client);

        _sut = new(_connectionFactory);

        _client.GetDatabase(Arg.Any<string>())
            .Returns(_db);
    }

    [Fact]
    public void RawVideoDb_ShouldCreateOneMongoDbConnection_WhenConstructed()
    {
        // Arrange
        var connectionFactory = Substitute.For<IMongoDbConnectionFactory>();

        // Act
        var _ = new RawVideoDb(connectionFactory);

        // Assert
        connectionFactory.Received(1).Create();
    }

    [Fact]
    public void GetCollection_ShouldGetCollectionFromSpecifiedDb_WhenSuppliedDbAndCollectionName()
    {
        // Arrange
        const string EXPECTED_DB_NAME = "test-db";
        const string EXPECTED_COLLECTION_NAME = "test-collection";

        var expectedCollection = Substitute.For<IMongoCollection<int>>();

        _db.GetCollection<int>(EXPECTED_COLLECTION_NAME)
            .Returns(expectedCollection);

        // Act
        var actualCollection = _sut.GetCollection<int>(EXPECTED_DB_NAME, EXPECTED_COLLECTION_NAME);

        // Assert
        _client.Received(1).GetDatabase(EXPECTED_DB_NAME);

        _db.Received(1).GetCollection<int>(EXPECTED_COLLECTION_NAME);

        actualCollection.Should().Be(expectedCollection);
    }

    [Fact]
    public void GetBucketFromDb_ShouldGetBucketFromSpecifiedDb_WhenSuppliedDbName()
    {
        // Arrange
        const string EXPECTED_DB_NAME = "test-db";

        // Act
        var bucket = _sut.GetBucketFromDb(EXPECTED_DB_NAME);

        // Assert
        _client.Received(1).GetDatabase(EXPECTED_DB_NAME);

        bucket.Should().NotBeNull();
    }

    private readonly RawVideoDb _sut;

    private readonly IMongoDbConnectionFactory _connectionFactory = Substitute.For<IMongoDbConnectionFactory>();
    private readonly MongoClientBase _client = Substitute.For<MongoClientBase>();
    private readonly IMongoDatabase _db = Substitute.For<IMongoDatabase>();
}
