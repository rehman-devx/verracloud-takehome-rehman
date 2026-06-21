import { useSelector } from "react-redux";

export default function PortfolioSummary() {
  const items = useSelector((state) => state.holdings.items);

  const totalMarketValue = items.reduce((sum, h) => sum + h.marketValue, 0);
  const totalPnL = items.reduce((sum, h) => sum + h.unrealizedPnL, 0);
  const totalPositions = items.length;

  const fmt = (n) =>
    n.toLocaleString("en-US", {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  const pnlPositive = totalPnL >= 0;

  return (
    <div className="grid grid-cols-3 gap-4 mb-6">
      <div className="bg-slate-800 rounded-lg p-4 border border-slate-700">
        <div className="text-xs text-slate-400 uppercase tracking-wider mb-2">
          Total Market Value
        </div>
        <div className="text-2xl font-bold text-white">
          ${fmt(totalMarketValue)}
        </div>
      </div>

      <div className="bg-slate-800 rounded-lg p-4 border border-slate-700">
        <div className="text-xs text-slate-400 uppercase tracking-wider mb-2">
          Unrealized P&L
        </div>
        <div
          className={`text-2xl font-bold ${pnlPositive ? "text-emerald-400" : "text-red-400"}`}
        >
          {pnlPositive ? "+" : "-"}${fmt(Math.abs(totalPnL))}
        </div>
      </div>

      <div className="bg-slate-800 rounded-lg p-4 border border-slate-700">
        <div className="text-xs text-slate-400 uppercase tracking-wider mb-2">
          Positions
        </div>
        <div className="text-2xl font-bold text-white">{totalPositions}</div>
      </div>
    </div>
  );
}
