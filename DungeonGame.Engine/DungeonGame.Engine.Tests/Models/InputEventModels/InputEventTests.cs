using System.Text.Json;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.Models.InputEventModels;

public class InputEventTests
{
    [Test]
    public void CanDeserializeEnergyDiceConfirmEvent()
    {
        var json = @"{
            ""EventType"": ""EnergyDiceConfirmAssignment""
        }";

        var parsed = JsonSerializer.Deserialize<InputEvent>(json, SerializationUtility.JsonSerializerOptions);

        Assert.That(parsed.EventType, Is.EqualTo(InputEventType.EnergyDiceConfirmAssignment));
        Assert.That(parsed.EventParameters, Is.Null);
    }

    [Test]
    public void CanDeserializeEvent()
    {
        var json = @"{
            ""EventType"": ""HeroActionMove"",
            ""EventParameters"": {
                ""X"": 2,
                ""Y"": 3
            }
        }";

        var parsed = JsonSerializer.Deserialize<InputEvent>(json, SerializationUtility.JsonSerializerOptions);

        Assert.That(parsed.EventType, Is.EqualTo(InputEventType.HeroActionMove));
        Assert.That(parsed.EventParameters, Is.Not.Null);

        var parsedEventParameters = parsed.EventParameters as HeroActionMoveEventParameters;
        Assert.That(parsedEventParameters.X, Is.EqualTo(2));
        Assert.That(parsedEventParameters.Y, Is.EqualTo(3));
    }

    [Test]
    public void CanDeserializeEnergyDiceAssignmentEvent()
    {
        var json = @"{
            ""EventType"": ""EnergyDiceAssign"",
            ""EventParameters"": {
                ""DiceIndex"": 1,
                ""SkillType"": ""Attack""
            }
        }";

        var parsed = JsonSerializer.Deserialize<InputEvent>(json, SerializationUtility.JsonSerializerOptions);

        Assert.That(parsed.EventType, Is.EqualTo(InputEventType.EnergyDiceAssign));
        Assert.That(parsed.EventParameters, Is.Not.Null);

        var parsedEventParameters = parsed.EventParameters as EnergyDiceAssignEventParameters;
        Assert.That(parsedEventParameters.DiceIndex, Is.EqualTo(1));
        Assert.That(parsedEventParameters.SkillType, Is.EqualTo(SkillType.Attack));
    }

    [Test]
    public void CanDeserializeHeroMoveEvent()
    {
        var json = @"{
            ""EventType"": ""HeroActionMove"",
            ""EventParameters"": {
                ""X"": 2,
                ""Y"": 4
            }
        }";

        var parsed = JsonSerializer.Deserialize<InputEvent>(json, SerializationUtility.JsonSerializerOptions);

        Assert.That(parsed.EventType, Is.EqualTo(InputEventType.HeroActionMove));
        Assert.That(parsed.EventParameters, Is.Not.Null);

        var parsedEventParameters = parsed.EventParameters as HeroActionMoveEventParameters;
        Assert.That(parsedEventParameters.X, Is.EqualTo(2));
        Assert.That(parsedEventParameters.Y, Is.EqualTo(4));
    }

    [Test]
    public void CanDeserializeHeroAttackEvent()
    {
        var json = @"{
            ""EventType"": ""HeroActionAttack"",
            ""EventParameters"": {
                ""X"": 3,
                ""Y"": 5
            }
        }";

        var parsed = JsonSerializer.Deserialize<InputEvent>(json, SerializationUtility.JsonSerializerOptions);

        Assert.That(parsed.EventType, Is.EqualTo(InputEventType.HeroActionAttack));
        Assert.That(parsed.EventParameters, Is.Not.Null);

        var parsedEventParameters = parsed.EventParameters as HeroActionAttackEventParameters;
        Assert.That(parsedEventParameters.X, Is.EqualTo(3));
        Assert.That(parsedEventParameters.Y, Is.EqualTo(5));
    }
}
