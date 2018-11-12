function parseEmbedCode() {
    var element = document.getElementById("embedCode");
    if (element.value.includes("tracks/") && element.value.includes("&color")) {
        var startCut = element.value.substring(element.value.indexOf("tracks/") + 7);        
        element.value = startCut.substring(0, startCut.indexOf("&color"));
    }
}