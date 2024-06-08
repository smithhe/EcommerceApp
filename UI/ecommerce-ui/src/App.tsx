//import './App.css'
import NavMenu from "./components/navmenu.tsx";
import {createBrowserRouter, createRoutesFromElements, Outlet, Route, RouterProvider} from "react-router-dom";
import Home from "./components/home.tsx";
import Categories from "./components/categories.tsx";
import Products from "./components/products.tsx";
import Login from "./components/security/login.tsx";
import Register from "./components/security/register.tsx";
import {AuthProvider} from "./AuthContext.tsx";
import ProductDetail from "./components/productdetail.tsx";
import Logout from "./components/security/logout.tsx";
import Cart from "./components/cart.tsx";
import Orders from "./components/Orders.tsx";

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route path="/" element={<RootLayout />}>
            <Route index element={<Home />} />
            <Route path="/categories" element={<Categories />}/>
            <Route path="/products/:categoryId" element={<Products />}/>
            <Route path="/productdetail/:productId" element={<ProductDetail />}/>
            <Route path="/login" element={<Login />} />
            <Route path="/logout" element={<Logout />} />
            <Route path="/register" element={<Register />} />
            <Route path="/cart" element={<Cart />}  />
            <Route path="/orders" element={<Orders />}  />
            <Route path="/checkout/success/:orderId" element={<Cart />}  />
        </Route>
    )
)

function RootLayout() {
    return (
        <AuthProvider>
            <div className="App">
                <NavMenu/>
                <div className="container container-bg">
                    <Outlet/>
                </div>
            </div>
        </AuthProvider>
    );
}

function App() {
    return <RouterProvider router={router}/>;
}

export default App
