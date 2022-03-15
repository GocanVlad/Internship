var Sdk = window.Sdk || {};
(function () {
  this.formOnLoad = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var client = formContext.getAttribute("aug_client").getValue();
    var tva = formContext.getAttribute("aug_tva").getValue();

    if (client) {
      formContext.getControl("aug_brand").setDisabled(false);
      formContext.getControl("aug_description").setDisabled(false);
    }
    if (!tva) formContext.getControl("aug_totalprice").setVisible(false);
  };

  saveClientMobileNumber = function (formcontext) {
    formcontext.data.save().then(function successSave() {
      Xrm.Navigation.openAlertDialog({
        text: "Record saved.",
      });
    });
  };

  this.clientOnChange = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var client = formContext.getAttribute("aug_client").getValue();
    var clientMobile = formContext.getAttribute("aug_clientmobilenumber");

    Xrm.WebApi.retrieveRecord(
      client[0].entityType,
      client[0].id,
      "?$select=telephone1,telephone2,address1_telephone1"
    ).then(function succes(result) {
      if (result.telephone1) {
        clientMobile.setValue(result.telephone1);
        formContext.getControl("aug_clientmobilenumber").setDisabled(false);
      } else if (result.telephone2) {
        clientMobile.setValue(result.telephone2);
        formContext.getControl("aug_clientmobilenumber").setDisabled(false);
      } else if (result.address1_telephone1) {
        clientMobile.setValue(result.address1_telephone1);
        formContext.getControl("aug_clientmobilenumber").setDisabled(false);
      } else {
        clientMobile.setValue("");
        formContext.getControl("aug_clientmobilenumber").setDisabled(true);
      }
    });
    saveClientMobileNumber(formContext);
  };

  this.tvaOnChange = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var tva = formContext.getAttribute("aug_tva").getValue();

    if (!tva || tva < 1 || tva > 25) {
      formContext.getControl("aug_totalprice").setVisible(false);
      formContext.ui.setFormNotification(
        "TVA Invalid",
        "ERROR",
        "formInvalidTva"
      );
      formContext
        .getControl("aug_tva")
        .setNotification("Valoare TVA poate fi intre 1 si 25", "invalidTva");
    } else {
      formContext.getControl("aug_totalprice").setVisible(true);
      formContext.ui.clearFormNotification("formInvalidTva");
      formContext.getControl("aug_tva").clearNotification("invalidTva");
    }
  };

  this.totalPriceOnChange = function (executionContext) {
    var formContext = executionContext.getFormContext();

    var unitPrice = formContext.getAttribute("aug_price").getValue();
    var quantity = formContext.getAttribute("aug_quantity").getValue();
    var tva = formContext.getAttribute("aug_tva").getValue();

    var quantityPrice = unitPrice * quantity;
    var totalPrice = quantityPrice + (tva * quantityPrice) / 100;

    formContext.getAttribute("aug_totalprice").setValue(totalPrice);

    if (!totalPrice) {
      formContext.ui.setFormNotification(
        "Total price invalid",
        "ERROR",
        "formInvalidTotalPrice"
      );
    } else formContext.ui.clearFormNotification("formInvalidTotalPrice");

    if (totalPrice < 1500 && tva != null) {
      var confirmStrings = {
        text: "Valoarea este mai mica de 1500. Doriti sa continuati salvarea datelor?",
        title: "Confirm",
        confirmButtonLabel: "Ok",
        cancelButtonLabel: "Cancel",
      };
      Xrm.Navigation.openConfirmDialog(confirmStrings, null).then(function (
        success
      ) {
        if (success.confirmed) formContext.data.save();
        else executionContext.getEventArgs().preventDefault();
      });
    }
  };

  this.formOnSave = function (executionContext) {};
}.call(Sdk));
