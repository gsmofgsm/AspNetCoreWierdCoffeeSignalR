// WebSocket = undefined;
//EventSource = undefined;
//, signalR.HttpTransportType.LongPolling

let connection = null;

setupConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/coffeehub")
        .build();

    connection.on("ReceiveOrderUpdate", (update) => {
            const statusDiv = document.getElementById("status");
            statusDiv.innerHTML = update;
        }
    );

    connection.on("NewOrder", function(order) {
            var statusDiv = document.getElementById("status");
            statusDiv.innerHTML = "Someone ordered an " + order.product;
        }
    );

    connection.on("finished", function() {
            connection.stop();
        }
    );

    // first create handlers of the various funcitons
    // then open connection
    // so you know no messages missing
    connection.start()
        .catch(err => console.error(err.toString())); 
};

setupConnection();

document.getElementById("submit").addEventListener("click", e => {
    e.preventDefault();
    const product = document.getElementById("product").value;
    const size = document.getElementById("size").value;

    fetch("/Coffee",
        {
            method: "POST",
            body: JSON.stringify({ product, size }),
            headers: {
               'content-type': 'application/json'
            }
        })
        .then(response => response.text())
        .then(id => connection.invoke("GetUpdateForOrder", id));
});