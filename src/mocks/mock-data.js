export const mockCompanies = [
    {
        id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1',
        name: 'Enerji Teknolojileri A.Ş.',
        projectName: 'AVM Elektrik Yönetimi',
        companyCode: 'AVM-001',
        taxNumber: '1234567890',
        address: 'Atatürk Mah. Çalışkan Cd. No:12, İstanbul',
        contact: 'info@enerjitek.com',
    },
    {
        id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2',
        name: 'Panel Yapı Ltd.',
        projectName: 'İşyeri Elektrik Yönetimi',
        companyCode: 'ISY-001',
        taxNumber: '9876543210',
        address: 'Gazi Bulv. No:45, İzmir',
        contact: 'iletisim@panelyapi.com',
    },
    {
        id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3',
        name: 'Elektrik Sistemleri Tic. Ltd.',
        projectName: 'Endüstriyel Elektrik Yönetimi',
        companyCode: 'END-001',
        taxNumber: '5647382910',
        address: 'Cumhuriyet Mh. Sanayi Sk. No:7, Ankara',
        contact: 'destek@elektriksistem.com',
    },
];
export const mockFacilities = [
    {
        id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
        companyId: mockCompanies[0].id,
        name: 'Merkez Üretim Tesisleri',
        city: 'İstanbul',
        district: 'Tuzla',
        address: 'Organize Sanayi Bölgesi, Parsel 34',
    },
    {
        id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2',
        companyId: mockCompanies[1].id,
        name: 'Depo ve Lojistik',
        city: 'İzmir',
        district: 'Bornova',
        address: 'Kargo Sok. No:8',
    },
    {
        id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3',
        companyId: mockCompanies[2].id,
        name: 'Ar-Ge Merkezi',
        city: 'Ankara',
        district: 'Çankaya',
        address: 'Teknokent Blok A, Kat 3',
    },
];
export const mockPanels = [
    {
        id: 'cccccccc-cccc-cccc-cccc-ccccccccccc1',
        facilityId: mockFacilities[0].id,
        code: 'P-001',
        name: 'Ana Dağıtım Paneli',
        description: 'Tesis ana dağıtım panosu, 400V, 1600A',
    },
    {
        id: 'cccccccc-cccc-cccc-cccc-ccccccccccc2',
        facilityId: mockFacilities[1].id,
        code: 'P-042',
        name: 'Üretim Hattı Paneli',
        description: 'Üretim hattı besleme panosu, 230V, 200A',
    },
    {
        id: 'cccccccc-cccc-cccc-cccc-ccccccccccc3',
        facilityId: mockFacilities[2].id,
        code: 'P-100',
        name: 'Test Laboratuvarı Paneli',
        description: 'Test laboratuvarı panosu, izolasyon ve ölçüm noktaları',
    },
];
export default {
    companies: mockCompanies,
    facilities: mockFacilities,
    panels: mockPanels,
};
