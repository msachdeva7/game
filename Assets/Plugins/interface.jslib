mergeInto(LibraryManager.library, {
  query_env: function (data) {
    var returnStr = window.unityBridge.query_env(Pointer_stringify(data));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  control: function (data) {
    var returnStr = window.unityBridge.control(Pointer_stringify(data));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
});
