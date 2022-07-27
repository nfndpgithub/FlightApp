// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/comHub").build();

connection.start().then(()=> console.log("connected")).catch((err) => console.log(er));



connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("sr").appendChild(li);
    
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
   li.textContent = `${user} says ${message}`;
});

function send() {
    let id = document.querySelector("[name=id]").value;
    let seats = document.querySelector("[name=seats]").value;

    if (id != "" && seats != "") {
        connection.invoke("SendMessage", id, seats).catch((err) => console.log(err));

    }
}

