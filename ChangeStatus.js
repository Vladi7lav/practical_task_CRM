function ChangeStatus() {
    var phoneNumber = Xrm.Page.getAttribute("new_phone_number_recipient").getValue();
    var message = Xrm.Page.getAttribute("new_message").getValue(); 
    var status = Xrm.Page.getAttribute("statuscode").getValue();
    //status: 100000000 - created, 100000001 - ready to send, 100000002 - sent
    if (phoneNumber === null || message === null || status === 100000001 || status === 100000002) return;
    else {
        Xrm.Page.getAttribute("statuscode").setValue(100000001);
    }
}