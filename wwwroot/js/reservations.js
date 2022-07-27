//const { signalR } = require("./signalr/dist/browser/signalr.js");
"use strict";

$(document).ready(() => {

   
    let $logBody = $("#logBody");
   
    });

const con = new signalR.HubConnectionBuilder().withUrl("/comHub").build();



con.on("SendRequest", data => {
    addFlight(data);

});
con.on("Answer", function (ids, answer) {
    var id = document.getElementById("ids");
    console.log(ids);
    console.log(id);
    let $td = $("#status");
    if (document.getElementById("status") == 'Pendindg') {
        var x = 'kkk';
        document.getElementById("status").innerHTML = x;
        console.log(document.getElementById("status"));
    }
    /*if (id == ids) {
        if (answer) {
            console.log(document.getElementById("status"));
            var x = 'kkk';
            //$td.textContent = 'kkk';
            //$td.append('kkk');
            document.getElementById("status").innerHTML = x;
        } else {
            document.getElementById("status").innerHTML("Declined");
        }

    } */
    else {
        console.log("stativa");
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
								<button type="submit" class="btn btn-success" value="accept" name="accept"> Accept</button>
								<button type="submit" class="btn btn-danger" value="decline" name="decline"> Decline</button>
							</td>

                    </tr>`;
    $logBody.append($(template));

    /////////////////////////////////////////////
   

    /*$logBody.append($(template));
    var tr = document.getElementById("logBody");
    tr.innerHTML = `<td>${data.Username}</td> 
                    <td>${data.From}</td>
                    <td>${data.To}</td>
                    <td>${data.Date}</td>
                    <td>${data.Capacity}</td>
                    <td>${data.Seats}</td>
                    <td>${data.Status}</td>
                    <td>${data.username}</td>`;
    tr.append();*/
    //var h1 = document.createElement("h1");
    //h1.innerHTML =`<tr>
     //       <td>${id}</td>
    //      </tr>`
    //tr.append(template);

    //$logBody.append($(template));

    //var td = document.createElement("td");
    //document.getElementById("logFlight").appendChild(td);
    //td.textContent = `${data.from}`;
    /*var tr = document.createElement("td");
    document.getElementById("row").appendChild(tr);
    tr.textContent = `${data.username} 
                    <td>${data.from}</td>
                    <td>${data.to}</td>
                    <td>${data.date}</td>
                    <td>${data.capacity}</td>
                    <td>${data.seats}</td>
                    <td>${data.status}</td>`;*/
    

    


}

con.start().catch(err => console.error(err.toString())); 
