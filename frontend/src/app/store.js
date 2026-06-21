import { configureStore } from "@reduxjs/toolkit";
import holdingsReducer from "../features/holdings/holdingsSlice.js";

export const store = configureStore({
  reducer: {
    holdings: holdingsReducer,
  },
});
