import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const API = "http://localhost:5283/api";

// Async thunks — these are like async action creators
export const fetchHoldings = createAsyncThunk("holdings/fetchAll", async () => {
  const response = await axios.get(`${API}/holdings`);
  return response.data;
});

export const addHolding = createAsyncThunk(
  "holdings/add",
  async (dto, { rejectWithValue }) => {
    try {
      const response = await axios.post(`${API}/holdings`, dto);
      return response.data;
    } catch (err) {
      return rejectWithValue(err.response.data.error);
    }
  },
);

export const deleteHolding = createAsyncThunk("holdings/delete", async (id) => {
  await axios.delete(`${API}/holdings/${id}`);
  return id;
});

export const fetchPrices = createAsyncThunk(
  "holdings/fetchPrices",
  async () => {
    const response = await axios.get(`${API}/prices`);
    return response.data;
  },
);

const holdingsSlice = createSlice({
  name: "holdings",
  initialState: {
    items: [],
    prices: [],
    status: "idle",
    error: null,
    addStatus: "idle",
    addError: null,
  },
  reducers: {
    // called by SignalR when prices update
    pricesUpdated(state, action) {
      state.prices = action.payload;
      // update current prices in holdings too
      state.items = state.items.map((holding) => {
        const price = action.payload.find((p) => p.ticker === holding.ticker);
        if (price) {
          const currentPrice = price.currentPrice;
          return {
            ...holding,
            currentPrice,
            marketValue: currentPrice * holding.quantity,
            unrealizedPnL:
              (currentPrice - holding.purchasePrice) * holding.quantity,
          };
        }
        return holding;
      });
    },
    clearAddError(state) {
      state.addError = null;
      state.addStatus = "idle";
    },
  },
  extraReducers: (builder) => {
    builder
      // fetch holdings
      .addCase(fetchHoldings.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchHoldings.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.items = action.payload;
      })
      .addCase(fetchHoldings.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      })
      // add holding
      .addCase(addHolding.pending, (state) => {
        state.addStatus = "loading";
        state.addError = null;
      })
      .addCase(addHolding.fulfilled, (state, action) => {
        state.addStatus = "succeeded";
        state.items.push(action.payload);
      })
      .addCase(addHolding.rejected, (state, action) => {
        state.addStatus = "failed";
        state.addError = action.payload;
      })
      // delete holding
      .addCase(deleteHolding.fulfilled, (state, action) => {
        state.items = state.items.filter((h) => h.id !== action.payload);
      })
      // fetch prices
      .addCase(fetchPrices.fulfilled, (state, action) => {
        state.prices = action.payload;
      });
  },
});

export const { pricesUpdated, clearAddError } = holdingsSlice.actions;
export default holdingsSlice.reducer;
