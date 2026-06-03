import { defineStore } from 'pinia';
import { reactive } from 'vue';
export const useModalStore = defineStore('modal', () => {
    const modals = reactive([]);
    function open(component, props, slots) {
        const id = Math.random().toString(36).slice(2, 9);
        modals.push({ id, component, props, slots });
        return id;
    }
    function close(id) {
        const i = modals.findIndex(m => m.id === id);
        if (i !== -1)
            modals.splice(i, 1);
    }
    function closeAll() { modals.splice(0, modals.length); }
    return { modals, open, close, closeAll };
});
