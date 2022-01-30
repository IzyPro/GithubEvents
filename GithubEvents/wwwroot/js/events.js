"use strict";

//This ensures wait time for the client and server to be connected before streaming
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};

//Creates a connection to the server hub
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/EventHub")
    .withAutomaticReconnect()
    .build();

//OnClick, streaming starts
document.getElementById("streamButton").addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
    try {
        connection.stream("EventsStream", 3000, 1)
            .subscribe({
                next: (item) => {
                    console.log(item);
                    document.getElementById("repoName").textContent = item.repo.name;
                    document.getElementById("repoLink").textContent = item.repo.url;
                    document.getElementById("eventAction").textContent = item.type;
                    document.getElementById("dt").textContent = item.created_at;
                    document.getElementById("owner").textContent = item.actor.display_login;
                    document.getElementById("isPublic").textContent = item.public ? "Public" : "Private";
                    document.getElementById("avatar").src = item.actor.avatar_url;
                    var li = document.createElement("li");
                    li.textContent = Item.type;
                    document.getElementById("messagesList").appendChild(li);
                },
                complete: () => {
                    document.getElementById("completeMsg").textContent = "End of Stream";
                },
                error: (err) => {
                    document.getElementById("errorMsg").textContent = err;
                },
            });
    }
    catch (e) {
        console.error(e.toString());
    }
    event.preventDefault();
}));

(() => __awaiter(this, void 0, void 0, function* () {
    try {
        yield connection.start();
    }
    catch (e) {
        console.error(e.toString());
    }
}))();

