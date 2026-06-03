export function formatCount(value) {
    return new Intl.NumberFormat('en-US').format(value);
}
export function formatDate(value) {
    return new Intl.DateTimeFormat('en-US', {
        year: 'numeric',
        month: 'short',
        day: '2-digit',
    }).format(new Date(value));
}
