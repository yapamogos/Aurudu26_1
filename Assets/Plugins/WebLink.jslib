mergeInto(LibraryManager.library, {
  OpenNewTab: function (url) {
    var urlString = UTF8ToString(url);
    window.open(urlString, '_blank');
  },
});