const urlParams = new URLSearchParams(window.location.search);
const cardType = urlParams.get('card') || 'master';

const menus = {
    master: {
        title: 'Master',
        items: ['User Management', 'Role Management', 'Department', 'Category']
    },
    reports: {
        title: 'Reports',
        items: ['Sales Report', 'Inventory Report', 'Stock Movement', 'Profit & Loss', 'Customer Report', 'Supplier Report', 'Tax Report', 'Custom Report']
    },
    settings: {
        title: 'Inventory',
        items: ['Add Item', 'View Items', 'Update Stock']
    }
};

const menu = menus[cardType];
// Keep Inex Tracker title, don't change it

const menuRoutes = {
    master: {
        'User Management': '/Master/UserManagement',
        'Role Management': '/Master/RoleManagement', 
        'Department': '/Master/Department',
        'Category': '/Master/Category'
    },
    reports: {
        'Sales Report': '/Reports/SalesReport',
        'Inventory Report': '/Reports/InventoryReport',
        'Stock Movement': '#',
        'Profit & Loss': '#',
        'Customer Report': '#',
        'Supplier Report': '#',
        'Tax Report': '#',
        'Custom Report': '#'
    },
    settings: {
        'Add Item': '/Inventory/AddItem',
        'View Items': '/Inventory/ViewItems',
        'Update Stock': '/Inventory/UpdateStock'
    }
};

const routes = menuRoutes[cardType] || {};
const menuHtml = menu.items.map(item => 
    `<div class="nav-item mb-1">
        <a class="nav-link text-white py-2 px-3 d-block rounded" href="${routes[item] || '#'}" style="background: rgba(255,255,255,0.1); margin-bottom: 5px;">${item}</a>
    </div>`
).join('');

document.getElementById('sidebarMenu').innerHTML = menuHtml;

function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const content = document.getElementById('content');
    const header = document.getElementById('header');
    sidebar.classList.toggle('show');
    content.classList.toggle('shifted');
    header.classList.toggle('shifted');
}

function searchMenu() {
    const searchBox = document.getElementById('searchBox');
    const menuItems = document.querySelectorAll('#sidebarMenu .nav-item');
    const searchTerm = searchBox.value.toLowerCase();
    
    menuItems.forEach(item => {
        const text = item.textContent.toLowerCase();
        if (text.includes(searchTerm)) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });
}