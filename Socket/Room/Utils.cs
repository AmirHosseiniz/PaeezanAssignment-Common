using System.Linq;

namespace Common.Socket.Room
{
    public class Utils
    {
        public static string[] GetSubTypes<T>()
        {
            var types = typeof(T).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(T))).ToArray();
            string[] subTypes = new string[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                subTypes[i] = types[i].FullName;
            }

            return subTypes;
        }
    }
}