import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { addHolding, clearAddError } from "../features/holdings/holdingsSlice";

export default function AddHoldingForm() {
  const dispatch = useDispatch();
  const { addStatus, addError } = useSelector((state) => state.holdings);
  const prices = useSelector((state) => state.holdings.prices);

  const [form, setForm] = useState({
    ticker: "",
    quantity: "",
    purchasePrice: "",
  });

  const [errors, setErrors] = useState({});

  const validate = () => {
    const e = {};
    if (!form.ticker.trim()) e.ticker = "Ticker is required";
    if (!form.quantity || Number(form.quantity) <= 0)
      e.quantity = "Quantity must be greater than 0";
    if (!form.purchasePrice || Number(form.purchasePrice) <= 0)
      e.purchasePrice = "Price must be greater than 0";
    return e;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const errs = validate();
    if (Object.keys(errs).length > 0) {
      setErrors(errs);
      return;
    }
    setErrors({});
    const result = await dispatch(
      addHolding({
        ticker: form.ticker.toUpperCase(),
        quantity: Number(form.quantity),
        purchasePrice: Number(form.purchasePrice),
      }),
    );
    if (result.meta.requestStatus === "fulfilled") {
      setForm({ ticker: "", quantity: "", purchasePrice: "" });
    }
  };

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    dispatch(clearAddError());
  };

  return (
    <div className="bg-slate-800 rounded-lg p-4 border border-slate-700 mb-6">
      <h2 className="text-sm font-semibold text-slate-300 uppercase tracking-wider mb-4">
        Add Holding
      </h2>

      <form onSubmit={handleSubmit} className="flex gap-3 items-end flex-wrap">
        <div className="flex flex-col gap-1">
          <label className="text-xs text-slate-400">Ticker</label>
          <select
            name="ticker"
            value={form.ticker}
            onChange={handleChange}
            className="bg-slate-900 border border-slate-600 rounded px-3 py-2 text-white text-sm w-28 focus:outline-none focus:border-blue-500"
          >
            <option value="">Select...</option>
            {prices.map((p) => (
              <option key={p.ticker} value={p.ticker}>
                {p.ticker}
              </option>
            ))}
          </select>
          {errors.ticker && (
            <span className="text-red-400 text-xs">{errors.ticker}</span>
          )}
        </div>

        <div className="flex flex-col gap-1">
          <label className="text-xs text-slate-400">Quantity</label>
          <input
            name="quantity"
            type="number"
            value={form.quantity}
            onChange={handleChange}
            placeholder="100"
            className="bg-slate-900 border border-slate-600 rounded px-3 py-2 text-white text-sm w-28 focus:outline-none focus:border-blue-500"
          />
          {errors.quantity && (
            <span className="text-red-400 text-xs">{errors.quantity}</span>
          )}
        </div>

        <div className="flex flex-col gap-1">
          <label className="text-xs text-slate-400">Purchase Price</label>
          <input
            name="purchasePrice"
            type="number"
            value={form.purchasePrice}
            onChange={handleChange}
            placeholder="150.00"
            className="bg-slate-900 border border-slate-600 rounded px-3 py-2 text-white text-sm w-32 focus:outline-none focus:border-blue-500"
          />
          {errors.purchasePrice && (
            <span className="text-red-400 text-xs">{errors.purchasePrice}</span>
          )}
        </div>

        <button
          type="submit"
          disabled={addStatus === "loading"}
          className="bg-blue-600 hover:bg-blue-700 disabled:bg-blue-900 disabled:text-blue-500 text-white text-sm font-medium px-4 py-2 rounded transition-colors"
        >
          {addStatus === "loading" ? "Adding..." : "Add Holding"}
        </button>

        {addError && <span className="text-red-400 text-sm">{addError}</span>}
      </form>
    </div>
  );
}
