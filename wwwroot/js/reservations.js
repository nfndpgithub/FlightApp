//const { signalR } = require("./signalr/dist/browser/signalr.js");
"use strict";

$(document).ready(() => {

   
    let $logBody = $("#logBody");
   
    });

const con = new signalR.HubConnectionBuilder().withUrl("/comHub").build();

con.start().catch(err => console.error(err.toString())); 


con.on("SendRequest", data => {
    addFlight(data);

});
con.on("Answer", function (ids, answer) {
    var id = ids;
    //console.log(id);
    
    var element = document.getElementById(ids);
    var row = document.getElementById(ids + "+2");
    console.log(row);
    if (answer) {
        element.innerHTML = "Accepted";
    } else {
        element.innerHTML = "Declined";
        row.style.backgroundColor = "#f23841";
    }
    
    
    
    
    
} )
function addFlight(data) {
    
    console.log(data);
    let $logBody = $("#logBody");
    
    
    let template = `<tr> 
                    <td>${data.username}</td> 
                    <td>${data.from}</td>
                    <td>${data.to}</td>
                    <td>${data.date}</td>
                    <td>${data.capacity}</td>
                    <td>${data.seats}</td>
                    <td>${data.status}</td>
                    
							<td>
								<input type="text" hidden value="@item.Id" name="id">
								<input type="text" hidden value="@item.Seats" name="seats">
								<button id="id" type="submit" class="btn btn-success" value="accept" name="accept" runat="server" OnServerClick="OnPostAnswer"> Accept</button>
								<button type="submit" class="btn btn-danger" value="decline" name="decline" onclick="on()"> Decline</button>
							</td>

                    </tr>`;
    $logBody.append($(template));

    /////////////////////////////////////////////
   

   


}
function on() {
    console.log("nesto");
    csAnswer();
}

function csAnswer() {
    $.ajax({
        type: "POST",
        url: 'AgentSide/OnPostAnswer',
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
           // $("#divResult").html("success");
        },
        error: function (e) {
            //$("#divResult").html("Something Wrong.");
        }
    });
}
