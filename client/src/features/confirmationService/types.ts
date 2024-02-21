type ConfirmationDialogOptions = {
  message?: string;
  successMessage?: string;
  title?: string;
  confirmLabel?: string;
  rejectLabel?: string;
  onConfirm?: () => void;
  onReject?: () => void;
  onConfirmError?: () => void;
  onRejectError?: () => void;
  closable?: boolean;
  closeWindowAfterConfirm?: boolean;
};

type ConfirmationStates = "none" | "error" | "loading" | "confirmed" | "rejected";
export type { ConfirmationDialogOptions, ConfirmationStates };
