import Swal from "sweetalert2";
const swallOptions = {
    confirmButtonColor: 'var(--primary-color)',
    cancelButtonColor: 'var(--secondary-color)',
    cancelButtonText: 'Отменить'
  };

const SwallPopup = Swal.mixin(swallOptions);

export default SwallPopup;