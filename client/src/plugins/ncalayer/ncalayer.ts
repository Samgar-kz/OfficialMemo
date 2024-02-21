import SwallPopup from "@/plugins/sweet-alert";
import { ref } from "vue";
var success = ref(false);
var connected = ref(false);
var iin = ref<string>();
var callback = null;
var webSocket = new WebSocket("wss://127.0.0.1:13579/");

webSocket.onopen = function (event) {
  connected.value = true;
  console.log("Connection opened");
};

webSocket.onclose = function (event) {
  if (event.wasClean) {
    console.log("connection has been closed");
  } else {
    connected.value = false;
    console.log("Connection error");
  }
  console.log("Code: " + event.code + " Reason: " + event.reason);
};

webSocket.onmessage = function (event) {
  var result = JSON.parse(event.data);

  if (result != null) {
    var rw = {
      code: result["code"],
      message: result["message"],
      responseObject: result["responseObject"],
      getResult: function () {
        return result;
      },
      getMessage: function () {
        return this.message;
      },
      getResponseObject: function () {
        return this.responseObject;
      },
      getCode: function () {
        return this.code;
      },
    };
    if (callback != null) {
      signXmlsBack(rw);
      callback = null;
    }
  }
};

function signXmlsCall() {
  if (connected.value) {
    SwallPopup.fire({
      title:
        "<div style='font-size: 22px;margin-top: 15px;'>Подождите, выполняется операция в NCALayer...</div>",
      allowEscapeKey: false,
      allowOutsideClick: false,
      showConfirmButton: false,
      didOpen: () => {
        SwallPopup.showLoading(SwallPopup.getDenyButton());
      },
    });

    var xmlToSign1 =
      "<signedData>" +
      b64EncodeUnicode(document.getElementById("document").outerText) +
      "</signedData>";
    var xmlsToSign = new Array(xmlToSign1);

    signXmls("PKCS12", "SIGNATURE", xmlsToSign, "signXmlsBack");
  } else {
    openDialog();
  }
}

function signXmls(
  storageName: string,
  keyType: string,
  xmlsToSign: string[],
  callBack: string
) {
  var signXmls = {
    module: "kz.gov.pki.knca.commonUtils",
    method: "signXmls",
    args: [storageName, keyType, xmlsToSign, "", ""],
  };
  callback = callBack;

  webSocket.send(JSON.stringify(signXmls));
}

function signXmlsBack(result: {
  [x: string]: any;
  code?: any;
  message?: any;
  responseObject?: any;
  getResult?: () => any;
  getMessage?: () => any;
  getResponseObject?: () => any;
  getCode?: () => any;
}) {
  SwallPopup.close();
  if (result["code"] === "500") {
    console.log(result["message"]);
  } else if (result["code"] === "200") {
    // var parser = new DOMParser();
    // var res = result["responseObject"];
    // var xmlDoc = parser.parseFromString(res[0], "text/xml");
    // const base64 =
    //   xmlDoc.getElementsByTagName("ds:X509Certificate")[0].childNodes[0]
    //     .nodeValue;
    // const bytes = Uint8Array.from(atob(base64), (b) => b.charCodeAt(0));
    // const byteStringBuffer = forge.util.createBuffer(bytes.buffer);
    // const asn1 = forge.asn1.fromDer(byteStringBuffer);
    // const cert = forge.pki.certificateFromAsn1(asn1);
    // var resultsTrue = cert.subject.attributes[2].value.toString();
    // iin.value = resultsTrue.substring(resultsTrue.indexOf("IIN") + 3, 15);
    // if (cert.validity.notAfter > new Date()) {
    success.value = true;
    // } else {
    //   SwallPopup.fire({
    //     title: `Истек срок действия ЭЦП!`,
    //     icon: "error",
    //   });
    // }
  }
}

function b64EncodeUnicode(str: any) {
  return btoa(
    encodeURIComponent(str).replace(
      /%([0-9A-F]{2})/g,
      function toSolidBytes(match, p1) {
        return String.fromCharCode(p1);
      }
    )
  );
}

function openDialog() {
  SwallPopup.fire({
    title: "<strong>Ошибка при подключении к NCALayer.</strong>",
    icon: "error",
    text: "Запустите NCALayer и нажмите ОК",
    showCancelButton: true,
    confirmButtonText: "ОК",
    showLoaderOnConfirm: true,
    preConfirm: async () => {
      location.reload();
    },
    allowOutsideClick: () => !SwallPopup.isLoading(),
  });
}

export { signXmlsCall, success, iin };
