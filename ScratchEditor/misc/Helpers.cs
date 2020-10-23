namespace ScratchEditor.misc
{
    public class Helpers
    {
        public static T GenT<T>() where T : new()
        {
            return new T();
        }
    }
}