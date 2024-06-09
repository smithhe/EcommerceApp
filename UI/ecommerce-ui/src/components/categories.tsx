import {useState, useEffect} from 'react';
import {useNavigate} from 'react-router-dom';
import {toast, ToastContainer} from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import {Category} from "../models/Category.ts";
import categoryService from "../services/CategoryService.ts";
import LoadingIcon from "./childComponents/LoadingIcon.tsx";

//The Categories page
const Categories = () => {
    const navigate = useNavigate();

    const [categoryList, setCategoryList] = useState<Category[] | undefined>(undefined);

    useEffect(() => {
        //Loads in the categories
        const fetchCategories = async () => {
            const response = await categoryService.getAllCategories();
            if (response.success) {
                setCategoryList(response.categories);
            } else {
                toast.error(response.message);
            }
        };

        fetchCategories().then(() => {
            console.log('fetched categories');
        });
    }, []);

    //Handles redirecting a user when they click on a category
    const onCategoryButtonClick = (categoryId: number) => {
        navigate(`/products/${categoryId}`);
    };

    //Default loading screen for when
    if (!categoryList) {
        return <LoadingIcon/>;
    }

    //Page to display once we have loaded in the category list
    return (
        <div className="container mt-5">
            <div className="header text-center">
                <h1 className="category-heading">Explore Our Categories</h1>
            </div>
            <div className="row mt-3">
                {categoryList.map((category, index) => (
                    <div className="col-md-4 mb-4" key={index}>
                        <div className="card">
                            <div className="card-body">
                                <h5 className="card-title">{category.name}</h5>
                                <p className="card-text">{category.summary}</p>
                                <a className="btn btn-primary" onClick={() => onCategoryButtonClick(category.id)}>View {category.name}</a>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            <ToastContainer/>
        </div>
    );
};

export default Categories;