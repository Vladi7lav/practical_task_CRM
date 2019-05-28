function Auto_Complete_Mobilephone() {
    var to = Xrm.Page.getAttribute("to").getValue();    
    var phonenumber = Xrm.Page.getAttribute("new_phone_number_recipient");
    if (to == null || to.length == 0) return;
    else
    {
        var Guid = to[0].id.substr(1, 36);
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/" + to[0].entityType + "s(" + Guid + ")?$select=telephone1", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState !== 4) return;
            else
            {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var result = JSON.parse(this.response);
                    phonenumber.setValue(result["telephone1"]);                        
                }
                else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();
    }
}
