using DiscordBot.Modules.SquadModule;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests;

public class SquadNameCheckerTests
{
    private readonly string[] _allowedWords = { "squad", "Team" };

    private SquadNameChecker _squadNameChecker = null!;

    [SetUp]
    public void Setup()
    {
        _squadNameChecker = new SquadNameChecker(_allowedWords);
    }

    [TestCase("SQUAD", true)]
    [TestCase("The super teaM", true)]
    [TestCase("teamPoop", true)]
    [TestCase("misspell sqaud", false)]
    [TestCase("aucun rapport", false)]
    public void Check_ShouldReturnCorrectly(string name, bool expected)
    {
        _squadNameChecker.CheckName(name).Should().Be(expected);
    }
}