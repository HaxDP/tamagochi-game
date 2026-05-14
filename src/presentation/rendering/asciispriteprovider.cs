using ConsoleTamagotchi.Domain.Enums;
using ConsoleTamagotchi.Domain.Models;

namespace ConsoleTamagotchi.Presentation.Rendering;

public sealed class AsciiSpriteProvider
{
    public AsciiSprite GetSprite(Pet pet, double elapsedSeconds)
    {
        if (pet.Stage == PetStage.Egg)
        {
            return GetEggSprite(elapsedSeconds);
        }

        return pet.AnimationState switch
        {
            PetAnimationState.Sleeping => GetSleepingSprite(pet.Species, pet.Stage, elapsedSeconds),
            PetAnimationState.Eating => GetEatingSprite(pet.Species, pet.Stage, elapsedSeconds),
            _ => GetIdleSprite(pet.Species, pet.Stage, elapsedSeconds)
        };
    }

    private static AsciiSprite GetEggSprite(double elapsedSeconds)
    {
        var crack = ((int)(elapsedSeconds * 3)) % 2 == 0;
        return crack
            ? new AsciiSprite([
                "   ____  ",
                "  / __ \\ ",
                " / /  \\ \\",
                "| | /\\ | |",
                " \\_\\__/ / "
            ])
            : new AsciiSprite([
                "   ____  ",
                "  / __ \\ ",
                " / / /\\ \\",
                "| | \\/ | |",
                " \\_\\__/ / "
            ]);
    }

    private static AsciiSprite GetIdleSprite(PetSpecies species, PetStage stage, double elapsedSeconds)
    {
        if (species == PetSpecies.Dog)
        {
            return GetDogIdleSprite(stage, elapsedSeconds);
        }

        var frame = ((int)(elapsedSeconds * 4)) % 2;
        return stage switch
        {
            PetStage.Baby => frame == 0
                ? new AsciiSprite([
                    "  /\\_/\\  ",
                    " ( o.o ) ",
                    "  > ^ <  "
                ])
                : new AsciiSprite([
                    "  /\\_/\\  ",
                    " ( o.o ) ",
                    "  > v <  "
                ]),
            PetStage.Teen => frame == 0
                ? new AsciiSprite([
                    " /\\___/\\ ",
                    "(  o o  )",
                    "(   ^   )",
                    " \\_____/"
                ])
                : new AsciiSprite([
                    " /\\___/\\ ",
                    "(  - -  )",
                    "(   ^   )",
                    " \\_____/"
                ]),
            PetStage.Adult => frame == 0
                ? new AsciiSprite([
                    " /\\_____/\\ ",
                    "(  o   o  )",
                    "(    ^    )",
                    "(  \\___/  )",
                    " \\_______/ "
                ])
                : new AsciiSprite([
                    " /\\_____/\\ ",
                    "(  -   -  )",
                    "(    ^    )",
                    "(  \\___/  )",
                    " \\_______/ "
                ]),
            _ => new AsciiSprite(["?"])
        };
    }

    private static AsciiSprite GetSleepingSprite(PetSpecies species, PetStage stage, double elapsedSeconds)
    {
        if (species == PetSpecies.Dog)
        {
            return GetDogSleepingSprite(stage, elapsedSeconds);
        }

        var z = ((int)(elapsedSeconds * 2)) % 3;
        var zText = z switch
        {
            0 => "z",
            1 => "zz",
            _ => "zzz"
        };

        return stage switch
        {
            PetStage.Baby => new AsciiSprite([
                "  /\\_/\\  ",
                " ( -.- ) " + zText,
                "  > _ <  "
            ]),
            PetStage.Teen => new AsciiSprite([
                " /\\___/\\ " + zText,
                "(  - -  )",
                "(   _   )",
                " \\_____/"
            ]),
            _ => new AsciiSprite([
                " /\\_____/\\ " + zText,
                "(  -   -  )",
                "(    _    )",
                "(  \\___/  )",
                " \\_______/ "
            ])
        };
    }

    private static AsciiSprite GetEatingSprite(PetSpecies species, PetStage stage, double elapsedSeconds)
    {
        if (species == PetSpecies.Dog)
        {
            return GetDogEatingSprite(stage, elapsedSeconds);
        }

        var bite = ((int)(elapsedSeconds * 5)) % 2 == 0 ? "*" : "o";
        return stage switch
        {
            PetStage.Baby => new AsciiSprite([
                "  /\\_/\\  ",
                $" ( ^.^ ) {bite}",
                "  > U <  "
            ]),
            PetStage.Teen => new AsciiSprite([
                " /\\___/\\ ",
                $"(  ^ ^  ) {bite}",
                "(   U   )",
                " \\_____/"
            ]),
            _ => new AsciiSprite([
                " /\\_____/\\ ",
                $"(  ^   ^  ) {bite}",
                "(    U    )",
                "(  \\___/  )",
                " \\_______/ "
            ])
        };
    }

    private static AsciiSprite GetDogIdleSprite(PetStage stage, double elapsedSeconds)
    {
        var frame = ((int)(elapsedSeconds * 4)) % 2;
        return stage switch
        {
            PetStage.Baby => frame == 0
                ? new AsciiSprite([
                    " / ^ ^ \\ ",
                    "(  o o  )",
                    " /  V  \\ "
                ])
                : new AsciiSprite([
                    " / ^ ^ \\ ",
                    "(  o o  )",
                    " /  U  \\ "
                ]),
            PetStage.Teen => frame == 0
                ? new AsciiSprite([
                    " / ^^^^^ \\ ",
                    "(  o   o  )",
                    "(    V    )",
                    " \\__---__/ "
                ])
                : new AsciiSprite([
                    " / ^^^^^ \\ ",
                    "(  -   -  )",
                    "(    V    )",
                    " \\__---__/ "
                ]),
            PetStage.Adult => frame == 0
                ? new AsciiSprite([
                    " / ^^^^^^^ \\ ",
                    "(  o     o  )",
                    "(    VVV    )",
                    "(  \\_____/  )",
                    " \\_________/ "
                ])
                : new AsciiSprite([
                    " / ^^^^^^^ \\ ",
                    "(  -     -  )",
                    "(    VVV    )",
                    "(  \\_____/  )",
                    " \\_________/ "
                ]),
            _ => new AsciiSprite(["?"])
        };
    }

    private static AsciiSprite GetDogSleepingSprite(PetStage stage, double elapsedSeconds)
    {
        var z = ((int)(elapsedSeconds * 2)) % 3;
        var zText = z switch
        {
            0 => "z",
            1 => "zz",
            _ => "zzz"
        };

        return stage switch
        {
            PetStage.Baby => new AsciiSprite([
                " / ^ ^ \\ " + zText,
                "(  - -  )",
                " /  _  \\ "
            ]),
            PetStage.Teen => new AsciiSprite([
                " / ^^^^^ \\ " + zText,
                "(  -   -  )",
                "(    _    )",
                " \\__---__/ "
            ]),
            _ => new AsciiSprite([
                " / ^^^^^^^ \\ " + zText,
                "(  -     -  )",
                "(    ___    )",
                "(  \\_____/  )",
                " \\_________/ "
            ])
        };
    }

    private static AsciiSprite GetDogEatingSprite(PetStage stage, double elapsedSeconds)
    {
        var bite = ((int)(elapsedSeconds * 5)) % 2 == 0 ? "*" : "o";
        return stage switch
        {
            PetStage.Baby => new AsciiSprite([
                " / ^ ^ \\ ",
                $"(  ^ ^  ) {bite}",
                " /  U  \\ "
            ]),
            PetStage.Teen => new AsciiSprite([
                " / ^^^^^ \\ ",
                $"(  ^   ^  ) {bite}",
                "(    U    )",
                " \\__---__/ "
            ]),
            _ => new AsciiSprite([
                " / ^^^^^^^ \\ ",
                $"(  ^     ^  ) {bite}",
                "(    UUU    )",
                "(  \\_____/  )",
                " \\_________/ "
            ])
        };
    }
}
