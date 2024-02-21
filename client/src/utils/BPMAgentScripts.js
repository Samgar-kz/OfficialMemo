const BPMAgent_Tkn = "20EABE5D64B0E216796E834F52D61FD0B70332FC";
const BPMAgent_NeedVersion = "BPM Agent v1.0.0";
const BPMAgent_HomeUrl = "http://localhost:7777/";
export function BPMAgent_CheckRun(ShowErrorAlert) {
  try {
    const CurrentVersion = BPMAgent_RunGetRequestSync("GetVersionInfo");
    if (CurrentVersion === BPMAgent_NeedVersion) return null;
    else {
      if (ShowErrorAlert)
        return (
          "Запушена некорректная версия BPMAgent\n на данный момент в вашем устройстве запущена " +
          CurrentVersion +
          ",\n для подписи необходима " +
          BPMAgent_NeedVersion
        );
    }
  } catch (e) {
    if (ShowErrorAlert) {
      console.log(e);
      return "BPMAgent не запущен на Вашем компьютере";
    }
  }
}
function BPMAgent_PhotoScanDocumentBase64InfoPathLocal(quality) {
  return BPMAgent_RunGetRequestSync("PhotoScanDocumentBase64InfoPathLocal?quality=" + quality);
}
function BPMAgent_PhotoScanDocumentBase64Local(quality) {
  return BPMAgent_RunGetRequestSync("PhotoScanDocumentBase64Local?quality=" + quality);
}
function BPMAgent_PhotoScanDocumentBase64InfoPathRemote(quality, webcamwsurl) {
  return BPMAgent_RunGetRequestSync("PhotoScanDocumentBase64InfoPathRemote?quality=" + quality + "&webcamwsurl=" + webcamwsurl);
}
function BPMAgent_PhotoScanDocumentBase64Remote(quality, webcamwsurl) {
  return BPMAgent_RunGetRequestSync("PhotoScanDocumentBase64Remote?quality=" + quality + "&webcamwsurl=" + webcamwsurl);
}
function BPMAgent_ScanDocumentBase64InfoPathLocal() {
  return BPMAgent_RunGetRequestSync("ScanDocumentBase64InfoPathLocal");
}
function BPMAgent_ScanDocumentBase64Local() {
  return BPMAgent_RunGetRequestSync("ScanDocumentBase64Local");
}
function BPMAgent_ScanDocumentBase64InfoPathRemote(webcamwsurl) {
  return BPMAgent_RunGetRequestSync("ScanDocumentBase64InfoPathRemote?webcamwsurl=" + webcamwsurl);
}
function BPMAgent_ScanDocumentBase64Remote(webcamwsurl) {
  return BPMAgent_RunGetRequestSync("ScanDocumentBase64Remote?webcamwsurl=" + webcamwsurl);
}
function BPMAgent_WebCamPhotoLocal(PhotoUrl, WebCamWSUrl) {
  let json = JSON.stringify({
    photourl: PhotoUrl,
    webcamwsurl: WebCamWSUrl,
  });
  return BPMAgent_RunPostRequestSync("WebCamPhotoLocal", json);
}
function BPMAgent_WebCamPhotoRemote(PhotoUrl, WebCamWSUrl) {
  let json = JSON.stringify({
    photourl: PhotoUrl,
    webcamwsurl: WebCamWSUrl,
  });
  return BPMAgent_RunPostRequestSync("WebCamPhotoRemote", json);
}
function BPMAgent_WebCamPhotoShowUploadDialog(PhotoUrl, WebCamWSUrl, IIN) {
  let json = JSON.stringify({
    photourl: PhotoUrl,
    webcamwsurl: WebCamWSUrl,
    iin: IIN,
  });
  return BPMAgent_RunPostRequestSync("WebCamPhotoShowUploadDialog", json);
}
function BPMAgent_ShowDepositDocPrinter(DocDir, EXEContent, XMLContent) {
  let jsonPrint = JSON.stringify({
    docdir: DocDir,
    execontent: EXEContent,
    xmlcontent: XMLContent,
  });
  BPMAgent_RunPostRequestSync("ShowDepositDocPrinter", jsonPrint);
}
function BPMAgent_FileReadFromTempFolderBase64(path) {
  return BPMAgent_RunPostRequestSync("FileReadFromTempFolderBase64", path);
}
function BPMAgent_FileSaveToTempFolderBase64(path, FileContent) {
  let jsonFile = JSON.stringify({
    path: path,
    content: FileContent,
  });
  return BPMAgent_RunPostRequestSync("FileSaveToTempFolderBase64", jsonFile);
}
function BPMAgent_FileReadBase64Local(path) {
  return BPMAgent_RunPostRequestSync("FileReadBase64Local", path);
}
function BPMAgent_FileReadTextLocal(path, encodingname) {
  let jsonFile = JSON.stringify({
    path: path,
    encodingname: encodingname,
  });
  return BPMAgent_RunPostRequestSync("FileReadTextLocal", jsonFile);
}
function BPMAgent_RunGetRequestSync(QueryString) {
  let xhr = new XMLHttpRequest();
  xhr.open("GET", BPMAgent_HomeUrl + QueryString, false);
  xhr.setRequestHeader("BpmToken", BPMAgent_Tkn);
  xhr.send();
  if (xhr.status === 200) return BPMAgent_CheckException(BPMAgent_RemoveQuotesFromResult(xhr.response));
  else {
    console.log("xhr.statusText: ", xhr.statusText);
    throw xhr.statusText;
  }
}
function BPMAgent_RunPostRequestSync(QueryString, content) {
  let xhr = new XMLHttpRequest();
  xhr.open("POST", BPMAgent_HomeUrl + QueryString, false);
  xhr.setRequestHeader("BpmToken", BPMAgent_Tkn);
  xhr.send(content);
  console.log(xhr);
  if (xhr.status === 200) return BPMAgent_CheckException(BPMAgent_RemoveQuotesFromResult(xhr.response));
  else throw xhr.statusText;
}
function BPMAgent_CheckException(inResult) {
  if (inResult.startsWith("BpmAgentException::")) throw new Error(inResult.slice(19));
  else return inResult;
}
function BPMAgent_RemoveQuotesFromResult(inResult) {
  if (inResult[0] === '"') inResult = inResult.slice(1);
  var n = inResult.length;
  if (inResult[n - 1] === '"') inResult = inResult.slice(0, n - 1);
  return inResult;
}
export function BPMAgent_SupervisorSign(files) {
  let jsonPrint = JSON.stringify({
    Files: files,
  });
  console.log("BPMAgent_SupervisorSign:", jsonPrint);
  var res = BPMAgent_RunPostRequestSync("SupervisorSign", jsonPrint);
  console.log("res: ", res);
  return res;
}
export function BPMAgent_OfficeSign(docNum, docAuthor, files, signFiles) {
  let jsonPrint = JSON.stringify({
    DocNum: docNum,
    DocAuthor: docAuthor,
    Files: files,
    SignFiles: signFiles,
    DocDate: getDateTimeNow(),
  });
  console.log("BPMAgent_OfficeSign:", jsonPrint);
  var res = BPMAgent_RunPostRequestSync("OfficeSign", jsonPrint);
  return res;
}

export function BPMAgent_RegisterDocument(docNum) {
  let jsonPrint = JSON.stringify({
    DocNum: docNum,
    DocDate: getDateTimeNow(),
  });
  var res = BPMAgent_RunPostRequestSync("RegisterDocument", jsonPrint);
  return res;
}

export function BPMAgent_SendIsNotValidReason(comment) {
  let jsonPrint = JSON.stringify({
    isNotValidReason: comment?.toString(),
  });
  const res = BPMAgent_RunPostRequestSync("SendIsNotValidReason", jsonPrint);
  return res;
}

export function getDateTimeNow() {
  const date = new Date();
  const day = date.getDate().toString().padStart(2, "0"); // Get the day and pad with leading zero if needed
  const month = (date.getMonth() + 1).toString().padStart(2, "0"); // Get the month and pad with leading zero if needed
  const year = date.getFullYear();

  const formattedDate = `${day}.${month}.${year}`;
  return formattedDate;
}
