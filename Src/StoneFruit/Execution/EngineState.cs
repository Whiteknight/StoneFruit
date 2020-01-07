namespace StoneFruit.Execution
{
    public class EngineState
    {
        public EngineState(bool headless)
        {
            Headless = headless;
            ShouldExit = false;
            CatchExceptions = true;
            PlaySound = false;
        }

        public bool ShouldExit { get; set; }
        public bool Headless { get; }

        public bool CatchExceptions { get; set; }
        public bool PlaySound { get; set; }
    }
}