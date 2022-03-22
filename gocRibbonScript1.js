var Sdk = window.Sdk || {};
(function () {
  this.mySampleFunction = function (primaryControl) {
    var formContext = primaryControl;
    var accountName = formContext.getControl("name").getAttribute().getValue();

    // Set the WebSiteURL column if account name contains "Contoso"
    if (accountName.toLowerCase().search("H&M") != -1) {
      formContext
        .getAttribute("websiteurl")
        .setValue("https://www.contoso.com");
    } else {
      Xrm.Navigation.openAlertDialog({
        text: "Account name does not contain 'Contoso'.",
      });
    }
  };
}.call(Sdk));
