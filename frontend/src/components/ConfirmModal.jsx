export default function ConfirmModal({ isOpen, onConfirm, onCancel, ticker }) {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
      <div className="bg-slate-800 border border-slate-700 rounded-lg p-6 w-full max-w-sm shadow-xl">
        <h3 className="text-white font-semibold text-lg mb-2">
          Delete Holding
        </h3>
        <p className="text-slate-400 text-sm mb-6">
          Are you sure you want to remove{" "}
          <span className="text-white font-bold">{ticker}</span> from your
          portfolio? This cannot be undone.
        </p>
        <div className="flex gap-3 justify-end">
          <button
            onClick={onCancel}
            className="px-4 py-2 text-sm text-slate-300 border border-slate-600 rounded hover:bg-slate-700 transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            className="px-4 py-2 text-sm text-white bg-red-600 rounded hover:bg-red-700 transition-colors"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  );
}
