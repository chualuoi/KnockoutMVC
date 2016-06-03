var koLib = koLib || {};
koLib.namespace = (function () {
    return function (namespaceString) {
        /// <summary>Create a namespace object</summary>
        /// <param name="namespaceString" type="String">namespace's name</param>
        var parts = namespaceString.split('.'),
            parent = window,
            currentPart;

        for (var i = 0, length = parts.length; i < length; i++) {
            currentPart = parts[i];
            parent[currentPart] = parent[currentPart] || { };
            parent = parent[currentPart];
        }

        return parent;
    };
})();