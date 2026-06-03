import { roleGroups } from '@/utils/authorization';
export const layoutMenu = [
    { label: 'Gosterge Paneli', icon: 'pi pi-home', to: { name: 'dashboard' }, roles: roleGroups.readOnly },
    { label: 'Sirketler', icon: 'pi pi-building', to: { name: 'companies' }, roles: roleGroups.readOnly },
    { label: 'Tesisler', icon: 'pi pi-map-marker', to: { name: 'facilities' }, roles: roleGroups.readOnly },
    { label: 'Paneller', icon: 'pi pi-box', to: { name: 'panels' }, roles: roleGroups.readOnly },
];
