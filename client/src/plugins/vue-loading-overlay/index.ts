import {useLoading} from 'vue-loading-overlay';
const options = {
    color: '#008b8a',
    loader: 'dots',
    width: 64,
    height: 64,
    backgroundColor: '#ffffff',
    opacity: 0.7,
    zIndex: 999,
};

const loadingOverlay = useLoading(options);

export default loadingOverlay;