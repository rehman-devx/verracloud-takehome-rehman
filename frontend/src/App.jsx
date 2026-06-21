import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { fetchHoldings, fetchPrices } from "./features/holdings/holdingsSlice";
import { startSignalRConnection } from "./services/signalr";
import PortfolioSummary from "./components/PortfolioSummary";
import HoldingsTable from "./components/HoldingsTable";
import AddHoldingForm from "./components/AddHoldingForm";

export default function App() {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(fetchHoldings());
    dispatch(fetchPrices());
    startSignalRConnection();

    const interval = setInterval(() => {
      dispatch(fetchHoldings());
    }, 5000);

    return () => clearInterval(interval);
  }, [dispatch]);

  return (
    <div className="min-h-screen bg-slate-900 text-slate-100 p-6">
      <div className="max-w-6xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-white">
            VerraCloud Bond Navigator
          </h1>
          <p className="text-slate-400 mt-1">Portfolio Holdings Dashboard</p>
        </div>

        <PortfolioSummary />
        <AddHoldingForm />
        <HoldingsTable />
      </div>
    </div>
  );
}
