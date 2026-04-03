using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.Utilities;

public class RandomUtilityTests
{
    [TestCase("37b16db7-1abc-44ba-a8a4-de5c39dbd2ea", 1, "4bf5bf72-6935-adb0-a19d-5b3547343bbe")]
    [TestCase("37b16db7-1abc-44ba-a8a4-de5c39dbd2ea", 2, "dd0dc972-3b83-621b-b7f3-f49d6ad0b6a6")]
    [TestCase("37b16db7-1abc-44ba-a8a4-de5c39dbd2ea", 3, "c8de4d87-f716-38b0-3ecd-4152103c6328")]
    [TestCase("37b16db7-1abc-44ba-a8a4-de5c39dbd2ea", 4, "2a307f0b-3cd5-d2a8-3a9f-4d7619a21bee")]
    [TestCase("4bf5bf72-6935-adb0-a19d-5b3547343bbe", 0, "1ea64ed1-318c-7c93-ee59-a0e46c5b3967")]
    [TestCase("4bf5bf72-6935-adb0-a19d-5b3547343bbe", 1, "d7c31668-1531-e7c1-018b-27690eb5f8c8")]
    [TestCase("4bf5bf72-6935-adb0-a19d-5b3547343bbe", 2, "46a16dc9-2442-d2f8-d3b1-78f7164f104e")]
    [TestCase("4bf5bf72-6935-adb0-a19d-5b3547343bbe", 3, "b1f43e2a-682c-ce85-8c84-4598a85834d3")]
    public void GivenGuidAndInt_ThenProduceGuidDeterministically(string guid, int number, string expected)
    {
        var actual = RandomUtility.GenerateDeterministicGuid(new Guid(guid), number);
        Assert.That(actual.ToString(), Is.EqualTo(expected));
    }
}