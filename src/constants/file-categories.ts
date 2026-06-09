export type PanelFileCategoryDefinition = {
  id?: string;
  key: string;
  label: string;
  title: string;
  description: string;
  icon: string;
  sortOrder?: number;
  isSystem?: boolean;
};

export const defaultPanelFileCategories: PanelFileCategoryDefinition[] = [
  {
    key: 'MaintenanceReport',
    label: 'Bakım',
    title: 'Bakım Raporu',
    description: 'Periyodik bakım kayıtları',
    icon: 'pi pi-wrench',
  },
  {
    key: 'ElectricalProject',
    label: 'Tek Hat',
    title: 'Tek Hat Dosyaları',
    description: 'Tek hat ve elektrik proje dosyaları',
    icon: 'pi pi-sitemap',
  },
  {
    key: 'PanelDocument',
    label: 'Proje',
    title: 'Proje Dosyaları',
    description: 'Yüklenen teknik proje dosyaları',
    icon: 'pi pi-file',
  },
];

export function getPanelFileCategoryDefinition(category: string): PanelFileCategoryDefinition {
  return defaultPanelFileCategories.find((item) => item.key === category) ?? {
    key: category,
    label: category,
    title: category,
    description: 'Panel dosyaları',
    icon: 'pi pi-file',
  };
}
