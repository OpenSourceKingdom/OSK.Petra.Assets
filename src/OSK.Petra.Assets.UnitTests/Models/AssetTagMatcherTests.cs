using OSK.Petra.Assets.Models;

namespace OSK.Petra.Assets.UnitTests;

public class AssetTagMatcherTests
{
    #region Matches

    [Fact]
    public void Matches_MatchingTag_ReturnsTrue()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            "TestCategory",
            [new AssetTagName("Tag1"), new AssetTagName("Tag2")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagName("Tag1"));

        // Act
        var result = matcher.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_NonMatchingTagCategory_ReturnsFalse()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            "TestCategory",
            [new AssetTagName("Tag1"), new AssetTagName("Tag2")]);

        var nonMatchingCategoryTag = new AssetTag(
            new AssetTagCategory("OtherCategory"),
            new AssetTagName("Tag2"));

        // Act
        var result = matcher.Matches(nonMatchingCategoryTag);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Matches_NonMatchingTagName_ReturnsFalse()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            "TestCategory",
            [new AssetTagName("Tag1"), new AssetTagName("Tag2")]);

        var nonMatchingNameTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagName("Tag3"));

        // Act
        var result = matcher.Matches(nonMatchingNameTag);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Matches_EmptyCategoryFilter_AllowsAnyCategory()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            "",
            [new AssetTagName("Tag1"), new AssetTagName("Tag2")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("AnyCategory"),
            new AssetTagName("Tag1"));

        // Act
        var result = matcher.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_EmptyTagNameFilter_AllowsAnyName()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            "TestCategory",
            []);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagName("AnyName"));

        // Act
        var result = matcher.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_CaseInsensitiveCategory_MatchesRegardlessOfCase()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            "testcategory",
            [new AssetTagName("tag1")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TESTCATEGORY"),
            new AssetTagName("tag1"));

        // Act
        var result = matcher.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_CaseInsensitiveTagName_MatchesRegardlessOfCase()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            "testcategory",
            [new AssetTagName("TAG1")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("testcategory"),
            new AssetTagName("tag1"));

        // Act
        var result = matcher.Matches(matchingTag);

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
        var matcher = new AssetTagMatcher(
            "TestCategory",
            [new AssetTagName(tagName)]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagName(tagName));

        // Act
        var result = matcher.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("tag1", "tag2")]
    [InlineData("tag1", "tag2", "tag3")]
    public void Matches_MultipleMatchingTags_ReturnsTrue(params string[] tagNames)
    {
        // Arrange
        var tagNamesConverted = new AssetTagName[tagNames.Length];
        for (int i = 0; i < tagNames.Length; i++)
        {
            tagNamesConverted[i] = new AssetTagName(tagNames[i]);
        }

        var matcher = new AssetTagMatcher("TestCategory", tagNamesConverted);

        var matchingTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagName(tagNames[0]));

        // Act
        var result = matcher.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Matches_NullOrWhitespaceTagName_ReturnsFalse()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            "TestCategory",
            [new AssetTagName("Tag1"), new AssetTagName("Tag2")]);

        var nullTagNameTag = new AssetTag(
            new AssetTagCategory("TestCategory"),
            new AssetTagName(""));

        // Act
        var result = matcher.Matches(nullTagNameTag);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Matches_NullCategoryFilter_AllowsAnyCategory()
    {
        // Arrange
        var matcher = new AssetTagMatcher(
            null!,
            [new AssetTagName("Tag1")]);

        var matchingTag = new AssetTag(
            new AssetTagCategory("AnyCategory"),
            new AssetTagName("Tag1"));

        // Act
        var result = matcher.Matches(matchingTag);

        // Assert
        Assert.True(result);
    }

    #endregion
}