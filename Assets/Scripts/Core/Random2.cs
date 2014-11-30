namespace UnityEngineExt {
    public static class Random2 {
        public static bool NextBool()
        {
            return UnityEngine.Random.Range(0, 100) < 50;
        }

        public static T RandomArrayElement<T>(T[] values)
        {
            int index = UnityEngine.Random.Range(0, values.Length);

            return values[index];
        }
    }
}
