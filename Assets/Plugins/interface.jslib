mergeInto(LibraryManager.library, {
  query_env: function (data) {
    var returnStr = window.unityBridge.query_env(Pointer_stringify(data));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  new_data: function (data, callback) {
    var returnStr = window.unityBridge.new_data(Pointer_stringify(data), function (returnStr) {
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        Runtime.dynCall('vi', callback, [buffer]);
    });
    return buffer;
  },
  end_level: function (data) {
    window.unityBridge.end_level(Pointer_stringify(data));
  },
});
