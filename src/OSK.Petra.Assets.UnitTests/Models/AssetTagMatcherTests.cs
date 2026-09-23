using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.UnitTests;

public class AssetTagFilterTests
{
    #region Matches

    [Fact]
    public void Matches_MatchingTag_ReturnsTrue()
    {
        // Arrange
        var filter = new AssetTagFilter(
            "TestCategory",
            [new AssetTagValue("Tag1"), new AssetTagValue("Tag2")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagValue("Tag1"));

        // Act
        var result = filter.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_NonMatchingTagCategory_ReturnsFalse()
    {
        // Arrange
        var filter = new AssetTagFilter(
            "TestCategory",
            [new AssetTagValue("Tag1"), new AssetTagValue("Tag2")]);

        var nonMatchingCategoryTag = new AssetTag(
            new AssetTagCategory("OtherCategory"),
            new AssetTagValue("Tag2"));

        // Act
        var result = filter.Matches(nonMatchingCategoryTag);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Matches_NonMatchingTagName_ReturnsFalse()
    {
        // Arrange
        var filter = new AssetTagFilter(
            "TestCategory",
            [new AssetTagValue("Tag1"), new AssetTagValue("Tag2")]);

        var nonMatchingNameTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagValue("Tag3"));

        // Act
        var result = filter.Matches(nonMatchingNameTag);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Matches_EmptyCategoryFilter_AllowsAnyCategory()
    {
        // Arrange
        var filter = new AssetTagFilter(
            "",
            [new AssetTagValue("Tag1"), new AssetTagValue("Tag2")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("AnyCategory"),
            new AssetTagValue("Tag1"));

        // Act
        var result = filter.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_EmptyTagNameFilter_AllowsAnyName()
    {
        // Arrange
        var filter = new AssetTagFilter(
            "TestCategory",
            []);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagValue("AnyName"));

        // Act
        var result = filter.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_CaseInsensitiveCategory_MatchesRegardlessOfCase()
    {
        // Arrange
        var filter = new AssetTagFilter(
            "testcategory",
            [new AssetTagValue("tag1")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TESTCATEGORY"),
            new AssetTagValue("tag1"));

        // Act
        var result = filter.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_CaseInsensitiveTagName_MatchesRegardlessOfCase()
    {
        // Arrange
        var filter = new AssetTagFilter(
            "testcategory",
            [new AssetTagValue("TAG1")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("testcategory"),
            new AssetTagValue("tag1"));

        // Act
        var result = filter.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("tag1")]
    [InlineData("tag2")]
    [InlineData("Tag3")]
    public void Matches_SingleMatchingTagName_ReturnsTrue(string tagName)
    {
        // Arrange
        var filter = new AssetTagFilter(
            "TestCategory",
            [new AssetTagValue(tagName)]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagValue(tagName));

        // Act
        var result = filter.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("tag1", "tag2")]
    [InlineData("tag1", "tag2", "tag3")]
    public void Matches_MultipleMatchingTags_ReturnsTrue(params string[] tagNames)
    {
        // Arrange
        var tagNamesConverted = new AssetTagValue[tagNames.Length];
        for (int i = 0; i < tagNames.Length; i++)
        {
            tagNamesConverted[i] = new AssetTagValue(tagNames[i]);
        }

        var filter = new AssetTagFilter("TestCategory", tagNamesConverted);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagValue(tagNames[0]));

        // Act
        var result = filter.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_NullOrWhitespaceTagName_ReturnsFalse()
    {
        // Arrange
        var filter = new AssetTagFilter(
            "TestCategory",
            [new AssetTagValue("Tag1"), new AssetTagValue("Tag2")]);

        var nullTagNameTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagValue(""));

        // Act
        var result = filter.Matches(nullTagNameTag);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Matches_NullCategoryFilter_AllowsAnyCategory()
    {
        // Arrange
        var filter = new AssetTagFilter(
            null!,
            [new AssetTagValue("Tag1")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("AnyCategory"),
            new AssetTagValue("Tag1"));

        // Act
        var result = filter.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    #endregion
}