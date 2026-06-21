import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { deleteHolding } from "../features/holdings/holdingsSlice";
import {
  Trash2,
  ChevronLeft,
  ChevronRight,
  ChevronUp,
  ChevronDown,
} from "lucide-react";
import ConfirmModal from "./ConfirmModal";

const PAGE_SIZE = 5;

export default function HoldingsTable() {
  const dispatch = useDispatch();
  const { items, status } = useSelector((state) => state.holdings);

  const [modal, setModal] = useState({ open: false, id: null, ticker: "" });
  const [page, setPage] = useState(1);
  const [tickerFilter, setTickerFilter] = useState("all");
  const [pnlFilter, setPnlFilter] = useState("all");
  const [sort, setSort] = useState({ column: null, direction: "asc" });

  const fmt = (n) =>
    n?.toLocaleString("en-US", {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });

  const handleSort = (column) => {
    setSort((prev) => ({
      column,
      direction:
        prev.column === column && prev.direction === "asc" ? "desc" : "asc",
    }));
    setPage(1);
  };

  const filtered = useMemo(() => {
    let result = [...items];

    // ticker filter
    if (tickerFilter !== "all") {
      result = result.filter((h) => h.ticker === tickerFilter);
    }

    // p&l filter
    if (pnlFilter === "winners") {
      result = result.filter((h) => h.unrealizedPnL > 0);
    } else if (pnlFilter === "losers") {
      result = result.filter((h) => h.unrealizedPnL < 0);
    }

    // sort
    if (sort.column) {
      result.sort((a, b) => {
        const aVal = a[sort.column];
        const bVal = b[sort.column];
        const dir = sort.direction === "asc" ? 1 : -1;
        return aVal > bVal ? dir : aVal < bVal ? -dir : 0;
      });
    }

    return result;
  }, [items, tickerFilter, pnlFilter, sort]);

  const totalPages = Math.ceil(filtered.length / PAGE_SIZE);
  const paginated = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  const handleDeleteClick = (holding) => {
    setModal({ open: true, id: holding.id, ticker: holding.ticker });
  };

  const handleConfirm = () => {
    dispatch(deleteHolding(modal.id));
    if (paginated.length === 1 && page > 1) setPage(page - 1);
    setModal({ open: false, id: null, ticker: "" });
  };

  const handleCancel = () => {
    setModal({ open: false, id: null, ticker: "" });
  };

  const handleFilterChange = (setter) => (e) => {
    setter(e.target.value);
    setPage(1); // reset to page 1 on filter change
  };

  const SortIcon = ({ column }) => {
    if (sort.column !== column)
      return <ChevronUp size={12} className="text-slate-600" />;
    return sort.direction === "asc" ? (
      <ChevronUp size={12} className="text-blue-400" />
    ) : (
      <ChevronDown size={12} className="text-blue-400" />
    );
  };

  const SortableHeader = ({ column, label, align = "right" }) => (
    <th
      className={`px-4 py-3 cursor-pointer hover:text-white transition-colors select-none text-${align}`}
      onClick={() => handleSort(column)}
    >
      <span className="flex items-center gap-1 justify-end">
        {label}
        <SortIcon column={column} />
      </span>
    </th>
  );

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
        {/* header + filters */}
        <div className="p-4 border-b border-slate-700 flex items-center justify-between flex-wrap gap-3">
          <h2 className="text-sm font-semibold text-slate-300 uppercase tracking-wider">
            Holdings
          </h2>

          <div className="flex items-center gap-2 flex-wrap">
            {/* ticker filter */}
            <select
              value={tickerFilter}
              onChange={handleFilterChange(setTickerFilter)}
              className="bg-slate-900 border border-slate-600 rounded px-3 py-1.5 text-white text-xs focus:outline-none focus:border-blue-500"
            >
              <option value="all">All Tickers</option>
              {[...new Set(items.map((h) => h.ticker))].sort().map((t) => (
                <option key={t} value={t}>
                  {t}
                </option>
              ))}
            </select>

            {/* p&l filter */}
            <select
              value={pnlFilter}
              onChange={handleFilterChange(setPnlFilter)}
              className="bg-slate-900 border border-slate-600 rounded px-3 py-1.5 text-white text-xs focus:outline-none focus:border-blue-500"
            >
              <option value="all">All P&L</option>
              <option value="winners">Winners only</option>
              <option value="losers">Losers only</option>
            </select>

            {/* result count */}
            {filtered.length > 0 && (
              <span className="text-xs text-slate-500">
                {filtered.length} of {items.length} holdings
              </span>
            )}
          </div>
        </div>

        {filtered.length === 0 ? (
          <div className="p-8 text-center text-slate-500">
            No holdings match your filters.
          </div>
        ) : (
          <>
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 uppercase tracking-wider border-b border-slate-700">
                  <th
                    className="text-left px-4 py-3 cursor-pointer hover:text-white transition-colors select-none"
                    onClick={() => handleSort("ticker")}
                  >
                    <span className="flex items-center gap-1">
                      Ticker <SortIcon column="ticker" />
                    </span>
                  </th>
                  <SortableHeader column="quantity" label="Quantity" />
                  <SortableHeader
                    column="purchasePrice"
                    label="Purchase Price"
                  />
                  <SortableHeader column="currentPrice" label="Current Price" />
                  <SortableHeader column="marketValue" label="Market Value" />
                  <SortableHeader
                    column="unrealizedPnL"
                    label="Unrealized P&L"
                  />
                  <th className="px-4 py-3"></th>
                </tr>
              </thead>
              <tbody>
                {paginated.map((holding, i) => {
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

            {/* pagination */}
            {totalPages > 1 && (
              <div className="px-4 py-3 border-t border-slate-700 flex items-center justify-between">
                <button
                  onClick={() => setPage((p) => p - 1)}
                  disabled={page === 1}
                  className="flex items-center gap-1 text-sm text-slate-400 hover:text-white disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
                >
                  <ChevronLeft size={16} />
                  Previous
                </button>

                <div className="flex gap-1">
                  {Array.from({ length: totalPages }, (_, i) => i + 1).map(
                    (p) => (
                      <button
                        key={p}
                        onClick={() => setPage(p)}
                        className={`w-7 h-7 text-xs rounded transition-colors ${
                          p === page
                            ? "bg-blue-600 text-white"
                            : "text-slate-400 hover:text-white hover:bg-slate-700"
                        }`}
                      >
                        {p}
                      </button>
                    ),
                  )}
                </div>

                <button
                  onClick={() => setPage((p) => p + 1)}
                  disabled={page === totalPages}
                  className="flex items-center gap-1 text-sm text-slate-400 hover:text-white disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
                >
                  Next
                  <ChevronRight size={16} />
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </>
  );
}
