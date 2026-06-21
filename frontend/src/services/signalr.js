import * as signalR from "@microsoft/signalr";
import { store } from "../app/store";
import { pricesUpdated } from "../features/holdings/holdingsSlice";

let connection = null;

export function startSignalRConnection() {
  connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5283/hubs/prices")
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

  // when server broadcasts price update
  connection.on("PricesUpdated", (prices) => {
    store.dispatch(pricesUpdated(prices));
  });

  connection.onreconnecting(() => {
    console.log("SignalR reconnecting...");
  });

  connection.onreconnected(() => {
    console.log("SignalR reconnected — refreshing data");
    store.dispatch(fetchHoldings());
  });

  connection.onclose(() => {
    console.log("SignalR connection closed");
  });

  connection
    .start()
    .then(() => console.log("SignalR connected"))
    .catch((err) => console.error("SignalR connection error:", err));
}

export function stopSignalRConnection() {
  connection?.stop();
}
