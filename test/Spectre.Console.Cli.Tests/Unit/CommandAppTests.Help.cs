namespace Spectre.Console.Tests.Unit.Cli;

public sealed partial class CommandAppTests
{
    [UsesVerify]
    [ExpectationPath("Help")]
    public class Help
    {
        [Fact]
        [Expectation("Root")]
        public Task Should_Output_Root_Correctly()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddCommand<DogCommand>("dog");
                configurator.AddCommand<HorseCommand>("horse");
                configurator.AddCommand<GiraffeCommand>("giraffe");
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Hidden_Commands")]
        public Task Should_Skip_Hidden_Commands()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddCommand<DogCommand>("dog");
                configurator.AddCommand<HorseCommand>("horse");
                configurator.AddCommand<GiraffeCommand>("giraffe")
                    .WithExample("giraffe", "123")
                    .IsHidden();
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Description_No_Trailing_Period")]
        public Task Should_Not_Trim_Description_Trailing_Period()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddCommand<DogCommand>("dog");
                configurator.AddCommand<HorseCommand>("horse");
                configurator.AddCommand<GiraffeCommand>("giraffe")
                    .WithExample("giraffe", "123")
                    .IsHidden();
                configurator.TrimTrailingPeriods(false);
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Command")]
        public Task Should_Output_Command_Correctly()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddBranch<CatSettings>("cat", animal =>
                {
                    animal.SetDescription("Contains settings for a cat.");
                    animal.AddCommand<LionCommand>("lion");
                });
            });

            // When
            var result = fixture.Run("cat", "--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Command_Hide_Default")]
        public Task Should_Not_Print_Default_Column()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddBranch<CatSettings>("cat", animal =>
                {
                    animal.SetDescription("Contains settings for a cat.");
                    animal.AddCommand<LionCommand>("lion");
                });
                configurator.HideOptionDefaultValues();
            });

            // When
            var result = fixture.Run("cat", "--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Leaf")]
        public Task Should_Output_Leaf_Correctly()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddBranch<CatSettings>("cat", animal =>
                {
                    animal.SetDescription("Contains settings for a cat.");
                    animal.AddCommand<LionCommand>("lion");
                });
            });

            // When
            var result = fixture.Run("cat", "lion", "--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Default")]
        public Task Should_Output_Default_Command_Correctly()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.SetDefaultCommand<LionCommand>();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Default_Without_Args")]
        public Task Should_Output_Default_Command_When_Command_Has_Required_Parameters_And_Is_Called_Without_Args()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.SetDefaultCommand<LionCommand>();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
            });

            // When
            var result = fixture.Run();

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Default_Without_Args_Additional")]
        public Task Should_Output_Default_Command_And_Additional_Commands_When_Default_Command_Has_Required_Parameters_And_Is_Called_Without_Args()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.SetDefaultCommand<LionCommand>();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddCommand<GiraffeCommand>("giraffe");
            });

            // When
            var result = fixture.Run();

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Greeter_Default")]
        public Task Should_Not_Output_Default_Command_When_Command_Has_No_Required_Parameters_And_Is_Called_Without_Args()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.SetDefaultCommand<GreeterCommand>();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
            });

            // When
            var result = fixture.Run();

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("RootExamples")]
        public Task Should_Output_Root_Examples_Defined_On_Root()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddExample("dog", "--name", "Rufus", "--age", "12", "--good-boy");
                configurator.AddExample("horse", "--name", "Brutus");
                configurator.AddCommand<DogCommand>("dog");
                configurator.AddCommand<HorseCommand>("horse");
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("RootExamples_Children")]
        public Task Should_Output_Root_Examples_Defined_On_Direct_Children_If_Root_Have_No_Examples()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddCommand<DogCommand>("dog")
                    .WithExample("dog", "--name", "Rufus", "--age", "12", "--good-boy");
                configurator.AddCommand<HorseCommand>("horse")
                    .WithExample("horse", "--name", "Brutus");
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("RootExamples_Leafs")]
        public Task Should_Output_Root_Examples_Defined_On_Leaves_If_No_Other_Examples_Are_Found()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddBranch<AnimalSettings>("animal", animal =>
                {
                    animal.SetDescription("The animal command.");
                    animal.AddCommand<DogCommand>("dog")
                        .WithExample("animal", "dog", "--name", "Rufus", "--age", "12", "--good-boy");
                    animal.AddCommand<HorseCommand>("horse")
                        .WithExample("animal", "horse", "--name", "Brutus");
                });
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("CommandExamples")]
        public Task Should_Only_Output_Command_Examples_Defined_On_Command()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddBranch<AnimalSettings>("animal", animal =>
                {
                    animal.SetDescription("The animal command.");
                    animal.AddExample(new[] { "animal", "--help" });

                    animal.AddCommand<DogCommand>("dog")
                        .WithExample("animal", "dog", "--name", "Rufus", "--age", "12", "--good-boy");
                    animal.AddCommand<HorseCommand>("horse")
                        .WithExample("animal", "horse", "--name", "Brutus");
                });
            });

            // When
            var result = fixture.Run("animal", "--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("DefaultExamples")]
        public Task Should_Output_Root_Examples_If_Default_Command_Is_Specified()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.SetDefaultCommand<LionCommand>();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddExample("12", "-c", "3");
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("NoDescription")]
        public Task Should_Not_Show_Truncated_Command_Table_If_Commands_Are_Missing_Description()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
                configurator.AddCommand<NoDescriptionCommand>("bar");
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("ArgumentOrder")]
        public Task Should_List_Arguments_In_Correct_Order()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.SetDefaultCommand<GenericCommand<ArgumentOrderSettings>>();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }

        [Fact]
        [Expectation("Hidden_Command_Options")]
        public Task Should_Not_Show_Hidden_Command_Options()
        {
            // Given
            var fixture = new CommandAppTester();
            fixture.SetDefaultCommand<GenericCommand<HiddenOptionSettings>>();
            fixture.Configure(configurator =>
            {
                configurator.SetApplicationName("myapp");
            });

            // When
            var result = fixture.Run("--help");

            // Then
            return Verifier.Verify(result.Output);
        }
    }
}