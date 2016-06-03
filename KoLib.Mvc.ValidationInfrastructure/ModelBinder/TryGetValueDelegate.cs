namespace KoLib.Mvc.ValidationInfrastructure.ModelBinder {
    internal delegate bool TryGetValueDelegate(object dictionary, string key, out object value);
}
