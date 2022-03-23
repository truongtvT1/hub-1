// This script exports photoshop layers as individual PNGs. It also
// writes a JSON file that can be imported into Spine where the images
// will be displayed in the same positions and draw order.

// Setting defaults.
var writePngs = true;

var imagesDir = "./images/";
var projectDir = "./json/";

var originalDoc;
try {
    originalDoc = app.activeDocument;
} catch (ignored) {
}

showDialog();

//run();
function run() {
    // Output dirs.
    var absProjectDir = absolutePath(projectDir);
    new Folder(absProjectDir).create();
    var absImagesDir = absolutePath(imagesDir);
    var imagesFolder = new Folder(absImagesDir);
    imagesFolder.create();
    activeDocument.duplicate();

    // Rasterize all layers.
    try {
        executeAction(stringIDToTypeID("rasterizeAll"), undefined, DialogModes.NO);
    } catch (ignored) {
    }

    var data = [];
    CollectData(activeDocument, data);
    storeHistory();
    var coor = GetAllPosition(absImagesDir, data);
    var result = ConvertDataToJSON(coor, data);
    var name = decodeURI(originalDoc.name);
    name = name.substring(0, name.indexOf("."));
    var file = new File(absProjectDir + name + ".json");
    file.remove();
    file.open("w", "TEXT");
    file.lineFeed = "\n";
    file.write(result);
    file.close();
	alert("Success!");
    // }
}

// Dialog and settings:

function showDialog() {
    if (!originalDoc) {
        alert("Please open a document before running the LayersToPNG script.");
        return;
    }
    if (!hasFilePath()) {
        alert("Please save the document before running the LayersToPNG script.");
        return;
    }

    var dialog = new Window("dialog", "Export PSD to Json for Unity UI");
    dialog.alignChildren = "fill";

    var checkboxGroup = dialog.add("group");
    var group = checkboxGroup.add("group");
    group.orientation = "column";
    group.alignChildren = "left";
    var writePngsCheckbox = group.add("checkbox", undefined, " Write layers as PNGs");
    writePngsCheckbox.value = writePngs;
    var outputGroup = dialog.add("panel", undefined, "Output directories");
    outputGroup.alignChildren = "fill";
    outputGroup.margins = [10, 15, 10, 10];
    var textGroup = outputGroup.add("group");
    group = textGroup.add("group");
    group.orientation = "column";
    group.alignChildren = "right";
    group.add("statictext", undefined, "Images:");
    group.add("statictext", undefined, "JSON:");
    group = textGroup.add("group");
    group.orientation = "column";
    group.alignChildren = "fill";
    group.alignment = ["fill", ""];
    group.add("statictext", undefined, imagesDir);
    group.add("statictext", undefined, projectDir);
    outputGroup.add("statictext", undefined, "Begin paths with \"./\" to be relative to the PSD file.").alignment = "center";

    var group = dialog.add("group");
    group.alignment = "center";
    var runButton = group.add("button", undefined, "OK");
    var cancelButton = group.add("button", undefined, "Cancel");
    cancelButton.onClick = function () {
        dialog.close(0);
        return;
    };

    runButton.onClick = function () {

        dialog.close(0);

        var rulerUnits = app.preferences.rulerUnits;
        app.preferences.rulerUnits = Units.PIXELS;
        try {
            run();
        } catch (e) {
            alert("An unexpected error has occurred.\n\nTo debug, run the LayersToPNG script using Adobe ExtendScript "
                + "with \"Debug > Do not break on guarded exceptions\" unchecked.");
            debugger;
        } finally {
            if (activeDocument != originalDoc) activeDocument.close(SaveOptions.DONOTSAVECHANGES);
            app.preferences.rulerUnits = rulerUnits;
        }
    };

    dialog.center();
    dialog.show();
}


var historyIndex;

function storeHistory() {
    historyIndex = activeDocument.historyStates.length - 1;
}

function restoreHistory() {
    activeDocument.activeHistoryState = activeDocument.historyStates[historyIndex];
}

function CollectData(doc, data) {
    for (var i = 0, n = doc.layers.length; i < n; i++) {
        var child = doc.layers[i];
        var obj = {
            target: child,
            children: []
        }
        if (child.layers && child.layers.length > 0) {
            obj.isDirectory = true;
            CollectData(child, obj.children);
            data.push(obj);
        } else if (child.kind == LayerKind.NORMAL) {
            child.visible = false;
            obj.isDirectory = false;
            data.push(obj);
        }

    }
}

function GetAllPosition(dir, data) {
    var count = data.length;
    var result = {width: 0, height: 0};
    for (var i = count - 1; i >= 0; i--) {
        var obj = data[i];
        if (!obj.isDirectory) {
            var layer = obj.target; // layer
            var attachmentName = layerName(layer);
            var x = activeDocument.width.as("px");
            var y = activeDocument.height.as("px");
            layer.visible = true;
            if (!layer.isBackgroundLayer) activeDocument.trim(TrimType.TRANSPARENT, false, true, true, false);
            else {
                result.width = activeDocument.width.as("px");
                result.height = activeDocument.height.as("px");
            }
            x -= activeDocument.width.as("px");
            y -= activeDocument.height.as("px");
            if (!layer.isBackgroundLayer) activeDocument.trim(TrimType.TRANSPARENT, true, false, false, true);
            var width = activeDocument.width.as("px");
            var height = activeDocument.height.as("px");
            if (writePngs) {
                activeDocument.saveAs(new File(dir + attachmentName), new PNGSaveOptions(), true, Extension.LOWERCASE);
            }
            restoreHistory();
            layer.visible = false;
            x += Math.round(width) / 2;
            y += Math.round(height) / 2;
            obj.x = x;
            obj.y = y;
            obj.width = width;
            obj.height = height;
        } else {
            obj.x = 0;
            obj.y = 0;
            obj.width = 0;
            obj.height = 0;
            GetAllPosition(dir, obj.children);
        }


    }
    return result;
}

function ConvertDataToJSON(coor, data) {
    var result = "{\"width\":" + coor.width + ",\"height\":" + coor.height + ",\"UI\":[";
    for (var i = 0; i < data.length; i++) {
        var obj = data[i];
        result += ConvertObjToJSON(obj);
        if (i < data.length - 1)
            result += ",";
    }
    result += "]}";
    return result;
}

function ConvertObjToJSON(obj) {
    var result = "{";
    result += "\"name\": " + "\"" + obj.target.name + "\",";
    result += "\"type\": " + (obj.isDirectory ? 1 : 0) + ",";
    result += "\"x\": " + obj.x + ",";
    result += "\"y\": " + obj.y + ",";
    result += "\"width\": " + obj.width + ",";
    result += "\"height\": " + obj.height + "";
    if (obj.children && obj.children.length > 0) {
        result += ",\"child\": [";
        for (var i = 0; i < obj.children.length; i++) {
            result += ConvertObjToJSON(obj.children[i]);
            if (i < obj.children.length - 1)
                result += ",";
        }
        result += "]";
    }
    result += "}";
    return result;
}

function hasFilePath() {
    var ref = new ActionReference();
    ref.putEnumerated(charIDToTypeID("Dcmn"), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
    return executeActionGet(ref).hasKey(stringIDToTypeID("fileReference"));
}

function absolutePath(path) {
    path = trim(path);
    if (path.length == 0)
        path = activeDocument.path.toString();
    else if (imagesDir.indexOf("./") == 0)
        path = activeDocument.path + path.substring(1);
    path = path.replace(/\\/g, "/");
    if (path.substring(path.length - 1) != "/") path += "/";
    return path;
}


function trim(value) {
    return value.replace(/^\s+|\s+$/g, "");
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function stripSuffix(str, suffix) {
    if (endsWith(str.toLowerCase(), suffix.toLowerCase())) str = str.substring(0, str.length - suffix.length);
    return str;
}

function layerName(layer) {
    return stripSuffix(trim(layer.name), ".png").replace(/[:\/\\*\?\"\<\>\|]/g, "");
}
