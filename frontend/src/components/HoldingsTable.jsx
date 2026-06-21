import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { deleteHolding } from "../features/holdings/holdingsSlice";
import { Trash2 } from "lucide-react";
import ConfirmModal from "./ConfirmModal";

export default function HoldingsTable() {
  const dispatch = useDispatch();
  const { items, status } = useSelector((state) => state.holdings);

  const [modal, setModal] = useState({ open: false, id: null, ticker: "" });

  const fmt = (n) =>
    n?.toLocaleString("en-US", {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });

  const handleDeleteClick = (holding) => {
    setModal({ open: true, id: holding.id, ticker: holding.ticker });
  };

  const handleConfirm = () => {
    dispatch(deleteHolding(modal.id));
    setModal({ open: false, id: null, ticker: "" });
  };

  const handleCancel = () => {
    setModal({ open: false, id: null, ticker: "" });
  };

  if (status === "loading") {
    return (
      <div className="bg-slate-800 rounded-lg border border-slate-700 p-8 text-center text-slate-400">
        Loading holdings...
      </div>
    );
  }

  if (status === "failed") {
    return (
      <div className="bg-slate-800 rounded-lg border border-slate-700 p-8 text-center text-red-400">
        Failed to load holdings. Is the backend running?
      </div>
    );
  }

  return (
    <>
      <ConfirmModal
        isOpen={modal.open}
        onConfirm={handleConfirm}
        onCancel={handleCancel}
        ticker={modal.ticker}
      />

      <div className="bg-slate-800 rounded-lg border border-slate-700 overflow-hidden">
        <div className="p-4 border-b border-slate-700">
          <h2 className="text-sm font-semibold text-slate-300 uppercase tracking-wider">
            Holdings
          </h2>
        </div>

        {items.length === 0 ? (
          <div className="p-8 text-center text-slate-500">
            No holdings yet. Add one above.
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="text-xs text-slate-400 uppercase tracking-wider border-b border-slate-700">
                <th className="text-left px-4 py-3">Ticker</th>
                <th className="text-right px-4 py-3">Quantity</th>
                <th className="text-right px-4 py-3">Purchase Price</th>
                <th className="text-right px-4 py-3">Current Price</th>
                <th className="text-right px-4 py-3">Market Value</th>
                <th className="text-right px-4 py-3">Unrealized P&L</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {items.map((holding, i) => {
                const pnlPositive = holding.unrealizedPnL >= 0;
                return (
                  <tr
                    key={holding.id}
                    className={`border-b border-slate-700/50 hover:bg-slate-700/30 transition-colors ${i % 2 === 0 ? "" : "bg-slate-700/10"}`}
                  >
                    <td className="px-4 py-3 font-bold text-white">
                      {holding.ticker}
                    </td>
                    <td className="px-4 py-3 text-right text-slate-300">
                      {holding.quantity}
                    </td>
                    <td className="px-4 py-3 text-right text-slate-300">
                      ${fmt(holding.purchasePrice)}
                    </td>
                    <td className="px-4 py-3 text-right text-slate-300">
                      ${fmt(holding.currentPrice)}
                    </td>
                    <td className="px-4 py-3 text-right text-slate-300">
                      ${fmt(holding.marketValue)}
                    </td>
                    <td
                      className={`px-4 py-3 text-right font-semibold ${pnlPositive ? "text-emerald-400" : "text-red-400"}`}
                    >
                      {pnlPositive ? "+" : "-"}$
                      {fmt(Math.abs(holding.unrealizedPnL))}
                    </td>
                    <td className="px-4 py-3 text-right">
                      <button
                        onClick={() => handleDeleteClick(holding)}
                        className="text-slate-500 hover:text-red-400 transition-colors"
                      >
                        <Trash2 size={16} />
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}
      </div>
    </>
  );
}
