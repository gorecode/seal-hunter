namespace UnityEngineExt {
    public static class Random2 {
        public static bool NextBool()
        {
            return UnityEngine.Random.Range(0, 100) < 50;
        }
    }
}
